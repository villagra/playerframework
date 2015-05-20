using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace Microsoft.VideoAdvertising
{
    /// <summary>
    /// Wraps a MAST Condition, applying the appropriate logic
    /// </summary>
    internal class ConditionManager : IDisposable
    {
        /// <summary>
        /// The condition we are managing
        /// </summary>
        public Condition Condition { get; protected set; }

        /// <summary>
        /// Our MAST Interface to player/system, events and properties
        /// </summary>
        public IMastAdapter MastInterface { get; protected set; }

        /// <summary>
        /// Our child conditions - these are treated as boolean 'AND'.  If we evaluate to true, we need to also check each child.
        /// </summary>
        public List<ConditionManager> Children = new List<ConditionManager>();

        public ConditionManager ParentCondition { get; set; }

        /// <summary>
        /// The event we'll fire to our Trigger parent if the event we are monitoring from the MAST Interface fires.
        /// </summary>
        public event EventHandler<object> EventFired;

        /// <summary>
        /// Track this so we can unwire our event
        /// </summary>
        Action eventUnhookAction;

        public bool IsEndCondition = false;

        public ConditionManager(Condition condition, IMastAdapter mastInterface)
        {
            if (condition == null)
            {
                throw new NullReferenceException("Condition must not be null");
            }
            if (mastInterface == null)
            {
                throw new NullReferenceException("IMastAdapter must not be null.");
            }

            Condition = condition;
            MastInterface = mastInterface;

            if (condition.Type == ConditionType.Event)
            {
                switch (condition.Name)
                { 
                    case "OnPlay":
                        MastInterface.OnPlay += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnPlay -= OnEventFired;
                        break;
                    case "OnStop":
                        MastInterface.OnStop += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnStop -= OnEventFired;
                        break;
                    case "OnPause":
                        MastInterface.OnPause += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnPause -= OnEventFired;
                        break;
                    case "OnMute":
                        MastInterface.OnMute += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnMute -= OnEventFired;
                        break;
                    case "OnVolumeChange":
                        MastInterface.OnVolumeChange += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnVolumeChange -= OnEventFired;
                        break;
                    case "OnEnd":
                        MastInterface.OnEnd += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnEnd -= OnEventFired;
                        break;
                    case "OnSeek":
                        MastInterface.OnSeek += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnSeek -= OnEventFired;
                        break;
                    case "OnItemStart":
                        MastInterface.OnItemStart += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnItemStart -= OnEventFired;
                        break;
                    case "OnItemEnd":
                        MastInterface.OnItemEnd += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnItemEnd -= OnEventFired;
                        break;
                    case "OnFullScreenChange":
                        MastInterface.OnFullScreenChange += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnFullScreenChange -= OnEventFired;
                        break;
                    case "OnPlayerSizeChanged":
                        MastInterface.OnPlayerSizeChanged += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnPlayerSizeChanged -= OnEventFired;
                        break;
                    case "OnError":
                        MastInterface.OnError += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnError -= OnEventFired;
                        break;
                    case "OnMouseOver":
                        MastInterface.OnMouseOver += OnEventFired;
                        eventUnhookAction = () => MastInterface.OnMouseOver -= OnEventFired;
                        break;
                }

                //ReflectionHelper.AttachEvent(MastInterface, condition.Name, new EventHandler(OnEventFired));
            }

            //Wire up our child conditions.  Note - only top level conditions can be based on events, or things would get weird.
            foreach (Condition c in Condition.Conditions)
            {
                if (c.Type == ConditionType.Event)
                {
                    throw new Exception("Event classed conditions can not be children");
                }
                ConditionManager cm = new ConditionManager(c, MastInterface) { ParentCondition = this };
                Children.Add(cm);
            }
        }

        public void DoOnEventFired()
        {
            OnEventFired(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fires from the MAST Interface event, if wired up
        /// </summary>
        public void OnEventFired(object sender, object args)
        {
            if (EventFired != null)
            {
                EventFired(this, args);
            }
        }

        /// <summary>
        /// Called by our parent to evaluate our condition
        /// </summary>
        /// <returns></returns>
        public bool Evaluate()
        {
            if (Condition == null) return false;

            //always return true here- this way we can properly evaluate children that may be property-based when our event fires.
            if (Condition.Type == ConditionType.Event) return true;

            //otherwise we know it's a property, get the value, we'll need it.
            object val = ReflectionHelper.GetValue(MastInterface, Condition.Name);

            //do a fancy comparison
            if (CompareValue(val, Condition.Value))
            {
                //check children - implicit 'AND'
                foreach (ConditionManager cm in Children)
                {
                    if (!cm.Evaluate()) return false;
                }

                //if none of our children failed, then we're still good.
                return true;
            }

            //if were here, compare returned false
            return false;
        }

        #region Type / value Comparisons

        /// <summary>
        /// Compare two values, with the appropriate operator
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool CompareValue(object prop, string val)
        {
            string typeName = prop.GetType().Name.ToLower();
            switch (typeName)
            {
                case "string":
                    return CompareString(prop, val);
                case "timespan":
                    return CompareTimeSpan(prop, val);
                case "datetime":
                    return CompareDateTime(prop, val);
                case "boolean":
                    return CompareBool(prop, val);
                default:
                    if (prop.GetType().GetTypeInfo().IsPrimitive)
                    {
                        //it's not bool or string, must be a number, right?
                        return CompareNumber(prop, val);
                    }
                    else
                    {
                        //unknown type
                        throw new Exception(string.Format("The property type '{0}' is unknown. ", prop.GetType().Name));
                    }
            }
        }

        private bool CompareNumber(object prop, string val)
        {
            double p = (Double)prop;
            double v = Convert.ToDouble(val);

            switch (Condition.Operator)
            {
                case Operator.EQ:
                    return (p == v);
                case Operator.NEQ:
                    return (p != v);
                case Operator.GTR:
                    return (p > v);
                case Operator.GEQ:
                    return (p >= v);
                case Operator.LT:
                    return (p < v);
                case Operator.LEQ:
                    return (p <= v);
                case Operator.MOD:
                    return (p % v == 0);
                default:
                    //unsupported operator
                    throw new Exception(string.Format("The operator {0} is not supported for '{1}' property types. ", Condition.Operator, prop.GetType().Name));
            }
        }

        private bool CompareString(object prop, string val)
        {
            int res = string.Compare(Condition.Value, val, StringComparison.CurrentCultureIgnoreCase);

            switch (Condition.Operator)
            {
                case Operator.EQ:
                    //TODO - make option for case sensitivity?  for now stick to ignoring case
                    return (res == 0);
                case Operator.NEQ:
                    return (res != 0);
                case Operator.GTR:
                    return (res > 0);
                case Operator.GEQ:
                    return (res >= 0);
                case Operator.LT:
                    return (res < 0);
                case Operator.LEQ:
                    return (res <= 0);
                default:
                    //unsupported operator
                    throw new Exception(string.Format("The operator {0} is not supported for '{1}' property types. ", Condition.Operator, prop.GetType().Name));
            }
        }

        private bool CompareTimeSpan(object prop, string val)
        {
            TimeSpan p = (TimeSpan)prop;
            TimeSpan v = TimeSpan.Parse(val);

            switch (Condition.Operator)
            {
                case Operator.EQ:
                    return (p == v);
                case Operator.NEQ:
                    return (p != v);
                case Operator.GTR:
                    return (p > v);
                case Operator.GEQ:
                    return (p >= v);
                case Operator.LT:
                    return (p < v);
                case Operator.LEQ:
                    return (p <= v);
                case Operator.MOD:
                    return ModTimeSpans(p, v);
                default:
                    //unsupported operator
                    throw new Exception(string.Format("The operator {0} is not supported for '{1}' property types. ", Condition.Operator, prop.GetType().Name));
            }
        }

        private bool CompareDateTime(object prop, string val)
        {
            DateTime p = (DateTime)prop;

            //special behavior for mod - compareing a timespan to a datetime
            if (Condition.Operator == Operator.MOD)
            {
                TimeSpan t = TimeSpan.Parse(val);
                ModTimeSpans(p.TimeOfDay, t);
            }

            DateTime v = DateTime.Parse(val);

            switch (Condition.Operator)
            {
                case Operator.EQ:
                    return (p == v);
                case Operator.NEQ:
                    return (p != v);
                case Operator.GTR:
                    return (p > v);
                case Operator.GEQ:
                    return (p >= v);
                case Operator.LT:
                    return (p < v);
                case Operator.LEQ:
                    return (p <= v);
                default:
                    //unsupported operator
                    throw new Exception(string.Format("The operator {0} is not supported for '{1}' property types. ", Condition.Operator, prop.GetType().Name));
            }
        }

        readonly List<CancellationTokenSource> cancellationTokens = new List<CancellationTokenSource>();
        private bool ModTimeSpans(TimeSpan prop, TimeSpan val)
        {
            TimeSpan remainder = prop;
            while (remainder >= val) remainder -= val;
            //exact match - but that's a slim chance, so give a little slop
            if (prop == val || prop - val <= TimeSpan.FromMilliseconds(100)) return true;

            if (prop > TimeSpan.FromMilliseconds(100))
            {
                var cancellationToken = new CancellationTokenSource();
                cancellationTokens.Add(cancellationToken);
                Task.Delay(prop, cancellationToken.Token).ContinueWith(t =>
                {
                    if (t.IsCompleted)
                    {
                        DoOnEventFired();
                    }
                    cancellationTokens.Remove(cancellationToken);
                    cancellationToken.Dispose();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            return false;
        }

        private bool CompareBool(object prop, string val)
        {
            switch (Condition.Operator)
            {
                case Operator.EQ:
                    //TODO - make option for case sensitivity?  for now stick to ignoring case
                    return ((bool)prop == ConversionHelper.ParseBool(val));
                case Operator.NEQ:
                    return ((bool)prop != ConversionHelper.ParseBool(val));
                default:
                    //unsupported operator
                    throw new Exception(string.Format("The operator {0} is not supported for '{1}' property types. ", Condition.Operator, prop.GetType().Name));
            }
        }

        #endregion

        public void Dispose()
        {
            foreach (var cancellationToken in cancellationTokens)
            {
                cancellationToken.Cancel();
                cancellationToken.Dispose();
            }
            cancellationTokens.Clear();

            if (eventUnhookAction != null)
            {
                eventUnhookAction();
                eventUnhookAction = null;
            }            

            foreach (ConditionManager cm in Children)
            {
                cm.Dispose();
            }
        }
    }
}
