$(document).ready(function () {
    const sideCart = $('#side-cart');
    const cartOverlay = $('#cart-overlay');
    const cartIcon = $('#cart-icon');


    function openCart() { sideCart.addClass('open'); cartOverlay.addClass('open'); }
    function closeCart() { sideCart.removeClass('open'); cartOverlay.removeClass('open'); }

    function updateCartView(cartHtml) {
        $('#cart-content').html(cartHtml);
        updateCartCount();
    }

    function updateCartCount() {
        var count = 0;
        $('#cart-content .cart-item').each(function () {
            var text = $(this).find('.item-price').text();
            var quantity = parseInt(text.split('x')[0].trim());
            if (!isNaN(quantity)) {
                count += quantity;
            }
        });
        $('#cart-count').text(count);
    }

    function loadCart() {
        $.get('/Cart/GetCartPartial', function (data) {
            updateCartView(data);
        });
    }

    loadCart();
    cartIcon.on('click', openCart);
    $('body').on('click', '.close-cart-btn, .close-cart-btn-bottom, .cart-overlay', closeCart);


    $('body').on('click', '.initial-add-btn', function (e) {
        e.preventDefault();
        const actionsContainer = $(this).closest('.product-actions');
        actionsContainer.find('.initial-add-container').hide();
        actionsContainer.find('.quantity-controls-container').show();
    });


    $('body').on('click', '.decrease-qty', function () {
        let qtyInput = $(this).siblings('.quantity-input');
        let currentVal = parseInt(qtyInput.val());
        if (currentVal > 1) {
            qtyInput.val(currentVal - 1);
        }
    });

    $('body').on('click', '.increase-qty', function () {
        let qtyInput = $(this).siblings('.quantity-input');
        qtyInput.val(parseInt(qtyInput.val()) + 1);
    });


    $('body').on('click', '.add-to-cart-btn', function () {
        const button = $(this);
        const actionsContainer = button.closest('.product-actions');
        const productId = button.data('product-id');
        const quantity = actionsContainer.find('.quantity-input').val();

        $.post('/Cart/AddToCart', { productId: productId, quantity: quantity }, function (data) {
            updateCartView(data);
            openCart();
            actionsContainer.find('.initial-add-container').show();
            actionsContainer.find('.quantity-controls-container').hide();
            actionsContainer.find('.quantity-input').val(1);
        }).fail(function () {
            alert("حدث خطأ أثناء إضافة المنتج.");
        });
    });

    $('body').on('click', '.remove-from-cart-btn', function () {
        var productId = $(this).data('product-id');
        $.post('/Cart/RemoveFromCart', { productId: productId }, function (data) {
            updateCartView(data);
        }).fail(function () {
            alert("حدث خطأ أثناء حذف المنتج.");
        });
    });
});
