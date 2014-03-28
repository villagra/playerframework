(function (PlayerFramework, undefined) {
    "use strict";

    // VpaidNonLinearAdViewModel Errors
    var invalidConstruction = "Invalid construction: VpaidNonLinearAdViewModel constructor must be called using the \"new\" operator.",
        invalidAdPlayer = "Invalid argument: VpaidNonLinearAdViewModel expects a VpaidAdPlayerBase as the first argument.";

    // VpaidNonLinearAdViewModel Class
    var VpaidNonLinearAdViewModel = WinJS.Class.derive(PlayerFramework.InteractiveViewModel, function (adPlayer, mediaPlayer) {
        if (!(this instanceof PlayerFramework.Advertising.VpaidNonLinearAdViewModel)) {
            throw invalidConstruction;
        }

        if (!(adPlayer instanceof PlayerFramework.Advertising.VpaidAdPlayerBase)) {
            throw invalidAdPlayer;
        }

        this._adPlayer = adPlayer;

        PlayerFramework.InteractiveViewModel.call(this, mediaPlayer);

        if (mediaPlayer.paused) {
            this._state = PlayerFramework.ViewModelState.paused;
        } else {
            this._state = PlayerFramework.ViewModelState.playing;
        }
    }, {
        // Public Methods
        playPause: function (e) {
            if (this._mediaPlayer.isPlayResumeAllowed) {
                this._mediaPlayer.playResume();
                this._adPlayer.resumeAd();
            } else {
                this._mediaPlayer.pause();
                this._adPlayer.pauseAd();
            }
        },

        playResume: function () {
            this._mediaPlayer.playResume();
            this._adPlayer.resumeAd();
        },

        pause: function () {
            this._mediaPlayer.pause();
            this._adPlayer.pauseAd();
        }
    });

    // VpaidNonLinearAdViewModel Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        VpaidNonLinearAdViewModel: VpaidNonLinearAdViewModel
    });

})(PlayerFramework);

