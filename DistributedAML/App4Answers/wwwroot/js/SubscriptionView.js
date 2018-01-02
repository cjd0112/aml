

$(function ($) {

    function SubscriptionView() {
        this.Parse = function() {
            alert("hit parse");
        }
    }

    window.SubscriptionView = new SubscriptionView();


})(jQuery);