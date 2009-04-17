﻿using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// allows 
    /// </summary>
    public class OffsetTracker
    {
        OffsetInfo _default = new OffsetInfo();
        string[] _ignore = new string[0];
        public OffsetInfo DefaultOffset { get { return _default; } set { _default = value; } }
        bool _ignoredefault = false;
        /// <summary>
        /// ignore symbols by default.   if true... a symbol has no custom offset defined will be ignored (regardless of ignore list).  the default is false.
        /// </summary>
        public bool IgnoreDefault { get { return _ignoredefault; } set { _ignoredefault = value; } }
        /// <summary>
        /// always ignore these symbols.   this list is only in affect when IgnoreDefault is false.
        /// </summary>
        public string[] IgnoreSyms { get { return _ignore; } set { _ignore = value; } }
        bool _hasevents = false;
        public event OrderDelegate SendOffset;
        public event UIntDelegate SendCancel;
        PositionTracker _pt = new PositionTracker();
        public PositionTracker PositionTracker { get { return _pt; } set { _pt = value; } }
        public OffsetTracker() { }
        public IdTracker _ids = new IdTracker();
        public IdTracker Ids { get { return _ids; } set { _ids = value; } }
        public OffsetTracker(uint initialid) : this(new IdTracker(initialid)) { }
        public OffsetTracker(IdTracker tracker)
        {
            _ids = tracker;
        }

        /// <summary>
        /// clear single custom offset
        /// </summary>
        /// <param name="sym"></param>
        public void ClearCustom(string sym) { _offvals.Remove(sym); }
        /// <summary>
        /// clear all custom offsets
        /// </summary>
        public void ClearCustom() { _offvals.Clear(); }


        void doupdate(string sym)
        {
            // is update ignored?
            if (IgnoreUpdate(sym)) return;
            // get our offset values
            OffsetInfo off = GetOffset(sym);
            // cancel existing profits
            cancel(off.ProfitId);
            // cancel existing stops
            cancel(off.StopId);
            // get new profit
            Order profit = Calc.PositionProfit(_pt[sym], off.ProfitDist, off.ProfitPercent,off.NormalizeSize,off.MinimumLotSize);
            // if it's valid, send and track it
            if (profit.isValid)
            {
                profit.id = Ids.AssignId;
                off.ProfitId = profit.id;
                SendOffset(profit);
            }
            // get new stop
            Order stop = Calc.PositionStop(_pt[sym], off.StopDist, off.StopPercent,off.NormalizeSize,off.MinimumLotSize);
            // if it's valid, send and track
            if (stop.isValid)
            {
                stop.id = Ids.AssignId;
                off.StopId = stop.id;
                SendOffset(stop);
            }
            // make sure new offset info is reflected
            SetOffset(sym, off);


        }

        bool hascustom(string symbol) { OffsetInfo oi; return _offvals.TryGetValue(symbol, out oi); }

        void cancel(OffsetInfo offset) { cancel(offset.ProfitId); cancel(offset.StopId); }
        void cancel(uint id) { if (id != 0) SendCancel(id); }
        /// <summary>
        /// cancels all offsets (profit+stops) for given side
        /// </summary>
        /// <param name="side"></param>
        public void CancelAll(bool side)
        {
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel all offsets for side
                if (p.isLong==side)
                    cancel(GetOffset(p.Symbol));
            }
        }
        /// <summary>
        /// cancels all offsets for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelAll(string sym)
        {
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.Symbol==sym)
                    cancel(GetOffset(sym));
            }
            
        }

        /// <summary>
        /// cancels only profit orders for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelProfit(string sym)
        {
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.Symbol == sym)
                    cancel(GetOffset(sym).ProfitId);
            }
        }

        /// <summary>
        /// cancels only stops for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelStop(string sym)
        {
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.Symbol == sym)
                    cancel(GetOffset(sym).StopId);
            }
        }

        /// <summary>
        /// cancel profits for side (long is true, false is short)
        /// </summary>
        /// <param name="side"></param>
        public void CancelProfit(bool side)
        {
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel profits for side
                if (p.isLong == side)
                    cancel(GetOffset(p.Symbol).ProfitId);
            }
        }

        /// <summary>
        /// cancel stops for a side (long is true, false is short)
        /// </summary>
        /// <param name="side"></param>
        public void CancelStop(bool side)
        {
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel stops for side
                if (p.isLong == side)
                    cancel(GetOffset(p.Symbol).StopId);
            }
        }

        /// <summary>
        /// cancels all tracked offsets
        /// </summary>
        public void CancelAll()
        {
            foreach (string sym in _offvals.Keys)
                cancel(GetOffset(sym));
        }


        bool HasEvents()
        {
            if (_hasevents) return true;
            if ((SendCancel == null) || (SendOffset == null))
                throw new Exception("You must define targets for SendCancel and SendOffset events.");
            _hasevents = true;
            return _hasevents;
        }

        bool IgnoreUpdate(string sym) 
        {
            // if updates are ignored by default
            if (_ignoredefault) // see if we have custom offset
                return !hascustom(sym);
            // otherwise see if it's specifically ignored
            foreach (string isym in _ignore) 
                if (sym == isym) 
                    return true; 
            return false; 
        }

        uint ProfitId(string sym)
        {
            OffsetInfo val;
            // if we have an offset, return the id
            if (_offvals.TryGetValue(sym, out val))
                return val.ProfitId;
            // no offset id
            return 0;
        }

        uint StopId(string sym)
        {
            OffsetInfo val;
            // if we have offset, return it's id
            if (_offvals.TryGetValue(sym, out val))
                return val.StopId;
            // no offset id
            return 0;
        }

        /// <summary>
        /// must send new positions here (eg from GotPosition on Response)
        /// </summary>
        /// <param name="p"></param>
        public void UpdatePosition(Position p)
        {
            // update position
            _pt.Adjust(p);
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(p.Symbol);
        }

        /// <summary>
        /// must send new fills here (eg call from Response.GotFill)
        /// </summary>
        /// <param name="t"></param>
        public void UpdatePosition(Trade t)
        {
            // update position
            _pt.Adjust(t);
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(t.symbol);

        }

        /// <summary>
        /// obtain curretn offset information for a symbol.
        /// if no custom value has been set, use default
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public OffsetInfo this[string symbol] { get { return GetOffset(symbol); } set { SetOffset(symbol, value); } }

        OffsetInfo GetOffset(string sym)
        {
            OffsetInfo val;
            // see if we have a custom offset
            if (_offvals.TryGetValue(sym, out val))
                return val;
            // otherwise use default
            return _default;
        }

        void SetOffset(string sym, OffsetInfo off)
        {
            OffsetInfo v;
            if (_offvals.TryGetValue(sym, out v))
                _offvals[sym] = off;
            else
                _offvals.Add(sym, off);
        }

        public void GotCancel(uint id)
        {
            // find any matching orders and reflect them as canceled
            foreach (string sym in _offvals.Keys)
            {
                if (_offvals[sym].StopId == id)
                    _offvals[sym].StopId = 0;
                else if (_offvals[sym].ProfitId == id)
                    _offvals[sym].ProfitId = 0;
            }

        }
        // track offset ids
        Dictionary<string, uint> _profitids = new Dictionary<string, uint>();
        Dictionary<string, uint> _stopids = new Dictionary<string, uint>();
        // per-symbol offset values
        Dictionary<string, OffsetInfo> _offvals = new Dictionary<string, OffsetInfo>();
        

    }

    public class OffsetInfo
    {
        public OffsetInfo(decimal profitdist, decimal stopdist, decimal profitpercent, decimal stoppercent, bool NormalizeSize, int MinSize)
        {
            ProfitDist = profitdist;
            StopDist = stopdist;
            ProfitPercent = profitpercent;
            StopPercent = stoppercent;
            this.NormalizeSize = NormalizeSize;
            MinimumLotSize = MinSize;
        }
        public bool NormalizeSize = false;
        public int MinimumLotSize = 1;
        public OffsetInfo(decimal profitdist, decimal stopdist) : this(profitdist,stopdist,1,1,false, 1) {}
        public OffsetInfo() : this(0,0,1,1,false,1) {}
        public uint ProfitId = 0;
        public uint StopId = 0;
        public decimal ProfitDist = 0;
        public decimal StopDist = 0;
        public decimal ProfitPercent = 1;
        public decimal StopPercent = 1;
    }
}