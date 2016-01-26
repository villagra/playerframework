using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
#if SILVERLIGHT
#else
using Windows.Storage.Streams;
using Windows.Foundation.Metadata;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.Media.Advertising
{
    /// <summary>
    /// The MAST Engine
    /// </summary>
    public sealed class Mainsail
    {
        #region Other Members
        /// <summary>
        /// A reference to the player. Trigger conditions are based on player properties and events.
        /// </summary>
        public IMastAdapter MastInterface { get; set; }

        /// <summary>
        /// The conditions for a trigger were met. The trigger is now active.
        /// </summary>
        public event EventHandler<TriggerEventArgs> ActivateTrigger;

        /// <summary>
        /// The conditions to deactivate a trigger were met. The trigger is no longer active.
        /// </summary>
        public event EventHandler<TriggerEventArgs> DeactivateTrigger;

        /// <summary>
        /// Evaluating the conditions for a trigger caused an error.
        /// </summary>
        public event EventHandler<TriggerFailureEventArgs> TriggerEvaluationFailed;

        /// <summary>
        /// The list of triggers we're monitoring
        /// </summary>
        private List<TriggerManager> Triggers = new List<TriggerManager>();
        private readonly List<TriggerManager> ActiveTriggers = new List<TriggerManager>();

        #endregion

        public Mainsail()
        { }

        public Mainsail(IMastAdapter mastInterface)
        {
            MastInterface = mastInterface;
        }
                
#if SILVERLIGHT
        public async Task LoadSource(Uri source, CancellationToken cancellationToken)
#else
        public IAsyncAction LoadSource(Uri source)
        {
            return AsyncInfo.Run(c => InternalLoadSource(source, c));
        }

        internal async Task InternalLoadSource(Uri source, CancellationToken cancellationToken)
#endif
        {
            using (var stream = await Extensions.LoadStreamAsync(source))
            {
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                await TaskEx.Run(() => AddMastDoc(stream), cancellationToken);
#elif WINDOWS_PHONE
                await Task.Run(() => AddMastDoc(stream), cancellationToken);
#else
                await Task.Run(() => AddMastDoc(stream.AsInputStream()), cancellationToken);
#endif
            }
        }

        public void EvaluateTriggers()
        {
            if (Triggers != null && Triggers.Count > 0)
            {
                foreach (TriggerManager tm in Triggers)
                {
                    Evaluate(tm);
                }
            }
        }

        private void Evaluate(TriggerManager tm)
        {
            try
            {
                //If it succeeds, it will take care of itself and fire event if ready
                tm.Evaluate();
            }
            catch (Exception ex)
            {
                TriggerEvaluationFailed(this, new TriggerFailureEventArgs(tm.Trigger, ex));
            }
        }

        private void tm_Activate(object sender, EventArgs e)
        {
            OnTriggerActivate(sender as TriggerManager);
        }

        void OnTriggerActivate(TriggerManager tm)
        {
            ActiveTriggers.Add(tm);

            if (ActivateTrigger != null)
                ActivateTrigger(this, new TriggerEventArgs(tm.Trigger));
        }

        private void tm_Deactivate(object sender, EventArgs e)
        {
            TriggerManager tm = sender as TriggerManager;
            OnTriggerDeactivate(tm);
        }

        internal void Deactivate(Trigger t)
        {
            var tm = ActiveTriggers.FirstOrDefault(at => at.Trigger == t);
            if (tm != null)
            {
                OnTriggerDeactivate(tm);
            }
        }

        /// <summary>
        /// Used to force the deactivation of a trigger
        /// </summary>
        /// <param name="t">The trigger that is to be deactivated</param>
        void OnTriggerDeactivate(TriggerManager tm)
        {
            ActiveTriggers.Remove(tm);

            if (DeactivateTrigger != null)
                DeactivateTrigger(this, new TriggerEventArgs(tm.Trigger));
        }

        #region Add/remove MAST docs and triggers


#if SILVERLIGHT
        public void AddMastDoc(Stream mastStream)
        {
            if (mastStream == null) throw new NullReferenceException("Mast stream cannot be null");

            var mast = MastModelFactory.CreateFromMast(mastStream);
            AddMastDoc(mast);
        }
#else
        [DefaultOverload()]
        public void AddMastDoc(IInputStream mastStream)
        {
            if (mastStream == null) throw new NullReferenceException("Mast stream cannot be null");

            var mast = MastModelFactory.CreateFromMast(mastStream);
            AddMastDoc(mast);
        }
#endif

        public void AddMastDoc(MAST mast)
        {
            if (mast == null || mast.Triggers == null)
            {
                throw new NullReferenceException("Mast doc/triggers cannot be null");
            }

            foreach (Trigger t in mast.Triggers)
            {
                AddMastTrigger(t);
            }
        }

        private void AddMastTrigger(Trigger t)
        {
            var oldtm = Triggers.FirstOrDefault(trig => trig.Trigger.Id == t.Id);

            if (oldtm != null)
            {
                if (ActiveTriggers.Contains(oldtm))
                {
                    // trigger with the same ID already exists and is currently active (in which case we don't want to update it or it could re-evaluate).
                    return;
                }
                else
                {
                    // trigger with the same ID already exists, replace it
                    Triggers.Remove(oldtm);
                    UnHookTrigger(oldtm);
                }
            }

            // add the new trigger
            TriggerManager tm = new TriggerManager(t, MastInterface);
            Triggers.Add(tm);
            HookUpTrigger(tm);
        }

        private void RemoveTrigger(TriggerManager tm)
        {
            lock (Triggers)
            {
                if (Triggers.Contains(tm))
                {
                    Triggers.Remove(tm);
                    UnHookTrigger(tm);
                }
            }
        }

        void HookUpTrigger(TriggerManager tm)
        {
            tm.Activate += tm_Activate;
            tm.Deactivate += tm_Deactivate;
        }

        void UnHookTrigger(TriggerManager tm)
        {
            tm.Activate -= tm_Activate;
            tm.Deactivate -= tm_Deactivate;
            tm.Dispose();
        }

        #endregion

        public void Clear()
        {
            foreach (TriggerManager t in Triggers.ToArray())
            {
                RemoveTrigger(t);
            }
        }
    }
}
