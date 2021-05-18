; (function ($, window, document, undefined) {

    /*This isn't entirely necessary, just playin around*/
    $('.textarea-maxlength').keyup(function () {

        var characterCount = $(this).val().length,
            current = $('.current-message'),
            maximum = $('.maximum-message'),
            theCount = $('.the-count-message');

        current.text(characterCount);

        /*This isn't entirely necessary, just playin around*/
        if (characterCount < 4000) {
            current.css('color', '#666');
        }
        if (characterCount > 4000 && characterCount < 6000) {
            current.css('color', '#6d5555');
        }
        if (characterCount > 6000 && characterCount < 8000) {
            current.css('color', '#793535');
        }
        if (characterCount > 8000 && characterCount < 9000) {
            current.css('color', '#841c1c');
        }
        if (characterCount > 9000 && characterCount < 10000) {
            current.css('color', '#8f0001');
        }

        if (characterCount >= 7000) {
            maximum.css('color', '#8f0001');
            current.css('color', '#8f0001');
            theCount.css('font-weight', 'bold');
        } else {
            maximum.css('color', '#666');
            theCount.css('font-weight', 'normal');
        }


    });

})(jQuery, window, document);