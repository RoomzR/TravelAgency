// Анимация при скролле
document.addEventListener('DOMContentLoaded', function() {
    // Плавная прокрутка для якорей
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                window.scrollTo({
                    top: targetElement.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Анимация появления элементов при скролле
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
            }
        });
    }, observerOptions);

    // Наблюдаем за элементами с классом .animate-on-scroll
    document.querySelectorAll('.animate-on-scroll').forEach(el => {
        observer.observe(el);
    });

    // Обработка форм с подтверждением
    document.querySelectorAll('form[data-confirm]').forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!confirm(this.getAttribute('data-confirm'))) {
                e.preventDefault();
            }
        });
    });

    // Динамическое обновление цены при выборе количества человек
    const peopleSelect = document.querySelector('select[name="peopleCount"]');
    const priceElement = document.querySelector('.total-price');
    
    if (peopleSelect && priceElement) {
        const basePrice = parseFloat(priceElement.dataset.basePrice || 0);
        
        peopleSelect.addEventListener('change', function() {
            const peopleCount = parseInt(this.value);
            const totalPrice = basePrice * peopleCount;
            priceElement.textContent = formatCurrency(totalPrice);
        });
    }

    // Форматирование валюты
    function formatCurrency(amount) {
        return new Intl.NumberFormat('ru-RU', {
            style: 'currency',
            currency: 'RUB',
            minimumFractionDigits: 0
        }).format(amount);
    }

    // Обработка добавления в избранное
    document.querySelectorAll('.wishlist-btn').forEach(btn => {
        btn.addEventListener('click', async function() {
            const tourId = this.dataset.tourId;
            const icon = this.querySelector('i');
            
            try {
                const response = await fetch(`/Wishlist/Toggle/${tourId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                
                if (response.ok) {
                    const result = await response.json();
                    
                    if (result.isInWishlist) {
                        icon.classList.remove('far');
                        icon.classList.add('fas');
                        this.classList.remove('btn-outline-primary');
                        this.classList.add('btn-danger');
                        showToast('Тур добавлен в избранное', 'success');
                    } else {
                        icon.classList.remove('fas');
                        icon.classList.add('far');
                        this.classList.remove('btn-danger');
                        this.classList.add('btn-outline-primary');
                        showToast('Тур удален из избранного', 'info');
                    }
                }
            } catch (error) {
                console.error('Error:', error);
                showToast('Ошибка при обновлении избранного', 'error');
            }
        });
    });

    // Toast уведомления
    function showToast(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white bg-${type} border-0`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');
        
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas fa-${getIconForType(type)} me-2"></i>
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" 
                        data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;
        
        const container = document.querySelector('.toast-container');
        if (!container) {
            const newContainer = document.createElement('div');
            newContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(newContainer);
            container = newContainer;
        }
        
        container.appendChild(toast);
        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();
        
        toast.addEventListener('hidden.bs.toast', function() {
            this.remove();
        });
    }
    
    function getIconForType(type) {
        switch (type) {
            case 'success': return 'check-circle';
            case 'error': return 'exclamation-circle';
            case 'warning': return 'exclamation-triangle';
            default: return 'info-circle';
        }
    }

    // Ленивая загрузка изображений
    if ('IntersectionObserver' in window) {
        const lazyImageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                    lazyImageObserver.unobserve(img);
                }
            });
        });
        
        document.querySelectorAll('img.lazy').forEach(img => {
            lazyImageObserver.observe(img);
        });
    }
});