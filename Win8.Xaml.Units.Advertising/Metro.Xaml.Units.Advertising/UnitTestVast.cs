using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.VideoAdvertising;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Microsoft.PlayerFramework.Units.Advertising
{
    [TestClass]
    public class UnitTestVast
    {
        AdHandlerController controller;

        public UnitTestVast()
        {
            controller = new AdHandlerController()
            {
                Player = new Mockups.Player()
            };
            controller.LoadPlayer += controller_LoadPlayer;
            controller.UnloadPlayer += controller_UnloadPlayer;
            controller.ActivateAdUnit += controller_ActivateAdUnit;
            controller.DeactivateAdUnit += controller_DeactivateAdUnit;
            controller.AdStateChanged += controller_AdStateChanged;
        }

        protected virtual IAdSource GetLinearAdSource()
        {
            return new Mockups.LocalAdSource()
            {
                Filename = @"Ads\vast_linear.xml",
                Type = "vast"
            };
        }

        AdState state;
        void controller_AdStateChanged(object sender, object e)
        {
            state = controller.AdState;
        }
        Stack<IVpaid> playerStack = new Stack<IVpaid>();
        Func<IVpaid> GetPlayer;
        void controller_LoadPlayer(object sender, LoadPlayerEventArgs e)
        {
            e.Player = GetPlayer();
            playerStack.Push(e.Player);
        }

        void controller_UnloadPlayer(object sender, UnloadPlayerEventArgs e)
        {
            Assert.AreSame(e.Player, playerStack.Pop());
        }

        Action OnActivate;
        Stack<ICreativeSource> adUnitStack = new Stack<ICreativeSource>();
        void controller_ActivateAdUnit(object sender, ActivateAdUnitEventArgs e)
        {
            if (OnActivate != null) OnActivate();
            adUnitStack.Push(e.CreativeSource);
        }

        void controller_DeactivateAdUnit(object sender, DeactivateAdUnitEventArgs e)
        {
            Assert.AreSame(e.CreativeSource, adUnitStack.Pop());
        }

        [TestMethod]
        public async Task Timeouts()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(2);
            OnActivate = null;
            GetPlayer = () => new Mockups.Vpaid();

            var adSource = GetLinearAdSource();

            var cancellationToken = new CancellationToken();
            var progress = new Progress<AdStatus>();

            // adsource timeout
            var timeoutAdSource = new Mockups.DelayAdSource(3000) { Type = "vast" };
            await UnitTestExtensions.ThrowsExceptionAsync<TimeoutException>(async () => await controller.PlayAdAsync(timeoutAdSource).AsTask(cancellationToken, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // NOTE: ad takes 4 seconds to start and 8 seconds to finish
            // initad timeout
            controller.StartTimeout = TimeSpan.FromSeconds(1);
            await UnitTestExtensions.ThrowsExceptionAsync<TimeoutException>(async () => await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // startad timeout
            controller.StartTimeout = TimeSpan.FromSeconds(3);
            await UnitTestExtensions.ThrowsExceptionAsync<TimeoutException>(async () => await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // timeout reached during playing (no exception thrown)
            controller.StartTimeout = TimeSpan.FromSeconds(5);
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // timeout reached during stopping (no exception thrown)
            controller.StartTimeout = TimeSpan.FromSeconds(7);
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // timeout reached after ad finishes playing (no exception thrown)
            controller.StartTimeout = TimeSpan.FromSeconds(10);
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            await Task.Delay(3000); // wait a little bit so timeout actually gets hit
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // no timeout specified
            controller.StartTimeout = null;
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }

        [TestMethod]
        public async Task Cancellation()
        {
            controller.StartTimeout = null;
            OnActivate = null;
            GetPlayer = () => new Mockups.Vpaid();

            var adSource = GetLinearAdSource();

            CancellationTokenSource cts;
            var progress = new Progress<AdStatus>();

            // adsource cancel
            var timeoutAdSource = new Mockups.DelayAdSource(3000) { Type = "vast" };
            cts = new CancellationTokenSource();
            cts.CancelAfter(2000);
            await UnitTestExtensions.ThrowsExceptionAsync<OperationCanceledException>(async () => await controller.PlayAdAsync(timeoutAdSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // NOTE: ad takes 4 seconds to start and 8 seconds to finish
            // initad cancel
            cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            await UnitTestExtensions.ThrowsExceptionAsync<OperationCanceledException>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // startad cancel
            cts = new CancellationTokenSource();
            cts.CancelAfter(3000);
            await UnitTestExtensions.ThrowsExceptionAsync<OperationCanceledException>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // playing cancel
            cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            await UnitTestExtensions.ThrowsExceptionAsync<OperationCanceledException>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // stopping cancel
            cts = new CancellationTokenSource();
            cts.CancelAfter(7000);
            await UnitTestExtensions.ThrowsExceptionAsync<OperationCanceledException>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // cancel after ad finishes playing (no exception thrown)
            cts = new CancellationTokenSource();
            cts.CancelAfter(10000);
            await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress);
            await Task.Delay(3000); // wait a little bit so timeout actually gets hit
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }

        [TestMethod]
        public async Task Errors()
        {
            controller.StartTimeout = null;
            OnActivate = null;

            var adSource = GetLinearAdSource();

            CancellationTokenSource cts = new CancellationTokenSource();
            var progress = new Progress<AdStatus>();

            // adsource cancel
            var errorAdSource = new Mockups.ErrorAdSource(new NotImplementedException()) { Type = "vast" };
            GetPlayer = () => { Assert.Fail(); return null; }; // this should not get hit
            await UnitTestExtensions.ThrowsExceptionAsync<NotImplementedException>(async () => await controller.PlayAdAsync(errorAdSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // handshake error: should not throw
            GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.Handshake };
            await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // initad error: should not throw
            //GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.Init };
            //await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.InitAsync };
            await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // startad error: should not throw
            //GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.Start };
            //await UnitTestExtensions.ThrowsExceptionAsync<NotImplementedException>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.StartAsync };
            await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // playing error: should not throw
            //GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.Play };
            //await UnitTestExtensions.ThrowsExceptionAsync<NotImplementedException>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.PlayAsync };
            await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // stopping error: should not throw
            //GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.Stop };
            //await UnitTestExtensions.ThrowsExceptionAsync<NotImplementedException>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            GetPlayer = () => new Mockups.Vpaid() { ErrorPlacement = Mockups.ErrorPlacement.StopAsync };
            await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cts.Token, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }

        [TestMethod]
        public async Task PlayAd()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(8);
            OnActivate = null;

            var adSource = GetLinearAdSource();

            var cancellationToken = new CancellationToken();
            var progress = new Progress<AdStatus>();

            GetPlayer = () => new Mockups.Vpaid();
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // play a synchronous ad
            GetPlayer = () => new Mockups.Vpaid() { Delay = null };
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }
        
        [TestMethod]
        public async Task NoHandler()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(2);
            OnActivate = () => Assert.Fail();
            GetPlayer = () => null;

            var adSource = new Mockups.AdSource() { Type = Guid.NewGuid().ToString() };
            var cancellationToken = new CancellationToken();
            var progress = new Progress<AdStatus>();

            await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }

        [TestMethod]
        public async Task CompanionFail()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(2);
            OnActivate = () => { throw new NotImplementedException(); };
            GetPlayer = () => new Mockups.Vpaid();

            var adSource = new Mockups.AdSource() { Type = Guid.NewGuid().ToString() };
            var cancellationToken = new CancellationToken();
            var progress = new Progress<AdStatus>();

            await UnitTestExtensions.ThrowsExceptionAsync<Exception>(async () => await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress));
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }
        
        [TestMethod]
        public async Task Preload()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(8);
            OnActivate = null;

            var adSource = GetLinearAdSource();

            var cancellationToken = new CancellationToken();

            GetPlayer = () => new Mockups.Vpaid();
            await controller.PreloadAdAsync(adSource).AsTask(cancellationToken);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            Assert.IsTrue(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // play the preloaded ad
            GetPlayer = () => { Assert.Fail(); throw new Exception(); };
            var progress = new Progress<AdStatus>();
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }

        [TestMethod]
        public async Task CancelPreload()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(8);
            OnActivate = null;

            var adSource = GetLinearAdSource();

            var cts = new CancellationTokenSource();

            GetPlayer = () => new Mockups.Vpaid();
            var task = controller.PreloadAdAsync(adSource).AsTask(cts.Token);
            cts.Cancel();
            await UnitTestExtensions.ThrowsExceptionAsync<OperationCanceledException>(async () => await task);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);
        }

        [TestMethod]
        public async Task PreloadAndPlayQuick()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(8);
            OnActivate = null;

            var adSource = GetLinearAdSource();

            var cancellationToken = new CancellationToken();

            GetPlayer = () => new Mockups.Vpaid();
            // don't wait
            var task = controller.PreloadAdAsync(adSource).AsTask(cancellationToken);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            await Task.WhenAny(task, Task.Delay(100));
            Assert.IsTrue(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // play the preloaded ad
            GetPlayer = () => { Assert.Fail(); throw new Exception(); };
            var progress = new Progress<AdStatus>();
            await controller.PlayAdAsync(adSource).AsTask(cancellationToken, progress);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public async Task PreloadAndPlayDifferentAd()
        {
            controller.StartTimeout = TimeSpan.FromSeconds(8);
            OnActivate = null;

            var adSource1 = GetLinearAdSource();
            var adSource2 = GetLinearAdSource();

            var cancellationToken = new CancellationToken();

            GetPlayer = () => new Mockups.Vpaid();
            // don't wait
            var task = controller.PreloadAdAsync(adSource1).AsTask(cancellationToken);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            await Task.WhenAny(task, Task.Delay(100));
            Assert.IsTrue(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            // play the preloaded ad
            var progress = new Progress<AdStatus>();
            await controller.PlayAdAsync(adSource2).AsTask(cancellationToken, progress);
            Assert.IsFalse(cancellationToken.IsCancellationRequested);
            Assert.IsFalse(playerStack.Any());
            Assert.IsTrue(state == AdState.None);

            Assert.IsTrue(task.IsCanceled);
        }
    }
}
