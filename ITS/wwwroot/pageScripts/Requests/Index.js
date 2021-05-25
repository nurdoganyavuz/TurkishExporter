;(function ($, window, document, undefined) {
	//Talepler sayfası devamını oku
	$('.devamyazi').on('click', function () {

		if ($(this).hasClass('aktif')) {
			$('.caticerik').css({ 'height': '20px', 'width': '300px', 'max-width': '200px' });
			$(this).removeClass('aktif');
		} else {
			$('.caticerik').css({ 'height': 'auto', 'width': '300px', 'max-width': '200px' });
			$(this).addClass('aktif');
		}
	});
	//Talepler sayfası devamını oku
})(jQuery, window, document);