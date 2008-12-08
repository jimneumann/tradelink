// TLServer_WM.cpp : implementation file
//

#include "stdafx.h"
#include "TLServer_WM.h"
#include "TradeLink.h"
#include "Util.h"
#include <fstream>
using namespace std;


namespace TradeLibFast
{

	TLServer_WM::TLServer_WM(void)
	{
		TLDEBUG = true;
		ENABLED = false;
		debugbuffer = CString("");
	}

	TLServer_WM::~TLServer_WM()
	{
		debugbuffer = "";
	}


	BEGIN_MESSAGE_MAP(TLServer_WM, CWnd)
		ON_WM_COPYDATA()
	END_MESSAGE_MAP()

	unsigned int TLServer_WM::FindClientFromStock(CString stock)
	{
		for (unsigned int i = 0; i<client.size(); i++)
			for (unsigned int j = 0; j<stocks[i].size(); j++)
			{
				if (stocks[i][j].CompareNoCase(stock)==0)
					return i;
			}
		return -1;
	}

	bool TLServer_WM::needStock(CString stock)
	{
		for (size_t i = 0; i<stocks.size(); i++)
			for (size_t j = 0; j<stocks[i].size(); j++)
			{
				if (stocks[i][j]==stock) return true;
			}
		return false;
	}

	int TLServer_WM::FindClient(CString cwind)
	{
		size_t len = client.size();
		for (size_t i = 0; i<len; i++) if (client[i]==cwind) return (int)i;
		return -1;
	}

	CString SerializeIntVec(std::vector<int> input)
	{
		std::vector<CString> tmp;
		for (size_t i = 0; i<input.size(); i++)
		{
			CString t; // setup tmp string
			t.Format("%i",input[i]); // convert integer into tmp string
			tmp.push_back(t); // push converted string onto vector
		}
		// join vector and return serialized structure
		return gjoin(tmp,",");
	}

	// TLServer_WM message handlers


	BOOL TLServer_WM::OnCopyData(CWnd* pWnd, COPYDATASTRUCT* pCopyDataStruct)
	{
		CString msg = (LPCTSTR)(pCopyDataStruct->lpData);
		int type = (int)pCopyDataStruct->dwData;
		return ServiceMessage(type,msg);
	}


	int TLServer_WM::RegisterClient(CString  clientname)
	{
		if (FindClient(clientname)!=-1) return OK;
		client.push_back(clientname);
		time_t now;
		time(&now);
		heart.push_back(now); // save heartbeat at client index
		clientstocklist my = clientstocklist(0);
		stocks.push_back(my);
		D(CString(_T("Client ")+clientname+_T(" connected.")));
		return OK;
	}

	int TLServer_WM::ServiceMessage(int MessageType, CString msg)
	{
			switch (MessageType)
			{
			case ORDERCANCELREQUEST :
				CancelRequest((long)atoi(msg.GetBuffer()));
				return OK;
			case ACCOUNTREQUEST :
				return AccountResponse(msg);
			case CLEARCLIENT :
				return ClearClient(msg);
			case CLEARSTOCKS :
				return ClearStocks(msg);
			case REGISTERSTOCK :
				{
				vector<CString> rec;
				gsplit(msg,CString("+"),rec);
				CString client = rec[0];
				vector<CString> hisstocks;
				// make sure client sent a basket, otherwise clear the basket
				if (rec.size()!=2) return ClearStocks(client);
				// get the basket
				gsplit(rec[1],CString(","),hisstocks);
				// make sure we have the client
				unsigned int cid = FindClient(client); 
				if (cid==-1) return CLIENTNOTREGISTERED; //client not registered
				// save the basket
				stocks[cid] = hisstocks; 
				D(CString(_T("Client ")+client+_T(" registered: ")+gjoin(hisstocks,",")));
				HeartBeat(client);
				return RegisterStocks(client);
				}
			case POSITIONREQUEST :
				{
				vector<CString> r;
				gsplit(msg,CString("+"),r);
				if (r.size()!=2) return BAD_PARAMETERS;
				return PositionResponse(r[1],r[0]);
				}
			case REGISTERCLIENT :
				return RegisterClient(msg);
			case HEARTBEAT :
				return HeartBeat(msg);
			case BROKERNAME :
				return BrokerName();
			case SENDORDER :
				return SendOrder(TLOrder::Deserialize(msg));
			case FEATUREREQUEST:
				{
					// get features supported by child class
					std::vector<int> stub = GetFeatures();
					// append basic feature we provide as parent
					stub.push_back(REGISTERCLIENT);
					stub.push_back(HEARTBEAT);
					stub.push_back(CLEARSTOCKS);
					stub.push_back(CLEARCLIENT);
					// send entire feature set back to client
					TLSend(FEATURERESPONSE,SerializeIntVec(stub),msg);
					return OK;
				}

			}
			return UnknownMessage(MessageType,msg);
	}

	int TLServer_WM::UnknownMessage(int MessageType, CString msg)
	{
		return UNKNOWNMSG;
	}

	int TLServer_WM::HeartBeat(CString clientname)
	{
			int cid = FindClient(clientname);
			if (cid==-1) return -1;
			time_t now;
			time(&now);
			time_t then = heart[cid];
			double dif = difftime(now,then);
			heart[cid] = now;
			return (int)dif;
	}

	int TLServer_WM::RegisterStocks(CString clientname) { return OK; }
	std::vector<int> TLServer_WM::GetFeatures() { std::vector<int> blank; return blank; } 

	int TLServer_WM::AccountResponse(CString clientname)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_WM::PositionResponse(CString account, CString clientname)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_WM::BrokerName(void)
	{
		return UnknownBroker;
	}

	int TLServer_WM::SendOrder(TLOrder order)
	{
		return FEATURE_NOT_IMPLEMENTED;
	}

	int TLServer_WM::ClearClient(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		client[cid] = "";
		stocks[cid] = clientstocklist(0);
		heart[cid] = NULL;
		D(CString(_T("Client ")+clientname+_T(" disconnected.")));
		return OK;
	}
	int TLServer_WM::ClearStocks(CString clientname)
	{
		int cid = FindClient(clientname);
		if (cid==-1) return CLIENTNOTREGISTERED;
		stocks[cid] = clientstocklist(0);
		HeartBeat(clientname);
		D(CString(_T("Cleared stocks for ")+clientname));
		return OK;
	}
	void TLServer_WM::SrvGotOrder(TLOrder order)
	{
		if (order.symbol=="") return;
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(ORDERNOTIFY,order.Serialize(),client[i]);
	}

	void TLServer_WM::D(const CString & message)
	{

		if (this->TLDEBUG)
		{
			const CString NEWLINE = "\r\n";
			CString line(message);
			line.Append(NEWLINE);
			debugbuffer.Append(line);
			__raise this->GotDebug(message);
		}
	}

	void TLServer_WM::SrvGotFill(TLTrade trade)
	{
		if (trade.symbol=="") return;
		for (size_t i = 0; i<stocks.size(); i++)
			for (size_t j = 0; j<stocks[i].size(); j++)
			{
				TLSecurity s = TLSecurity::Deserialize(stocks[i][j]);
				if (s.sym==trade.symbol)
					TLSend(EXECUTENOTIFY,trade.Serialize(),client[i]);
			}
	}

	void TLServer_WM::SrvGotTick(TLTick tick)
	{
		if (tick.sym=="") return;
		for (uint i = 0; i<stocks.size(); i++)
			for (uint j = 0; j<stocks[i].size(); j++)
			{
				if (stocks[i][j]==tick.sym)
					TLSend(TICKNOTIFY,tick.Serialize(),client[i]);
			}
	}

	void TLServer_WM::SrvGotCancel(int orderid)
	{
		CString id;
		id.Format(_T("%i"),orderid);
		for (size_t i = 0; i<client.size(); i++)
			if (client[i]!="")
				TLSend(ORDERCANCELRESPONSE,id,client[i]);
	}

	void TLServer_WM::CancelRequest(long order)
	{
		return;
	}

	bool TLServer_WM::HaveSubscriber(CString stock)
	{
		for (size_t i = 0; i<stocks.size(); i++) // go through each client
			for (size_t j = 0; j<stocks[i].size(); j++) // and each stock
				if (stocks[i][j].CompareNoCase(stock)==0) 
					return true;
		return false;
	}
	const char* VERFILE = "c:\\progra~1\\tradelink\\brokerserver\\VERSION.txt";
	void TLServer_WM::Start(bool live)
	{
		if (!ENABLED)
		{
			CString major = "0.1";
			CString minor("$Rev: 197 $");
			std::ifstream file;
			file.open(VERFILE);
			if (file.is_open())
			{
				char data[8];
				file.getline(data,8);
				minor = CString(data);
				file.close();
			}
			else
			{
				minor.Replace("$Rev: ","");
				minor.TrimRight(" $");
			}
			CString wind(live ? LIVEWINDOW : SIMWINDOW);
			this->Create(NULL, wind, 0,CRect(0,0,20,20), CWnd::GetDesktopWindow(),NULL);
			this->ShowWindow(SW_HIDE); // hide our window
			CString msg;
			msg.Format("Started TL BrokerServer %s [ %s.%s]",wind,major,minor);
			this->D(msg);
			ENABLED = true;
		}
	}


	int TLServer_WM::TLSend(int type,LPCTSTR msg,CString windname) 
	{
		LRESULT result = 999;
		HWND dest = FindWindowA(NULL,(LPCSTR)(LPCTSTR)windname)->GetSafeHwnd();
		
		if (dest) 
		{
			COPYDATASTRUCT CD;  // windows-provided structure for this purpose
			CD.dwData=type;		// stores type of message
			int len = 0;
			len = (int)strlen((char*)msg);

			CD.cbData = len+1;
			CD.lpData = (void*)msg;	//here's the data we're sending
			result = ::SendMessageA(dest,WM_COPYDATA,0,(LPARAM)&CD);
		} 
		return (int)result;
	}
}









