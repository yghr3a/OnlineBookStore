window.showAlert = function (message, duration = 4000) {
    const container = document.getElementById('global-alert');
    const messageBox = document.getElementById('alert-message');
    const progressBar = container.querySelector('.alert-progress');
    const closeBtn = document.getElementById('alert-close');

    messageBox.textContent = message;
    container.classList.remove('hidden');

    // 重置动画
    progressBar.style.animation = 'none';
    void progressBar.offsetWidth; // 强制重绘
    progressBar.style.animation = `alert-progress-bar ${duration / 1000}s linear forwards`;

    const timeout = setTimeout(() => {
        container.classList.add('hidden');
    }, duration);

    closeBtn.onclick = () => {
        clearTimeout(timeout);
        container.classList.add('hidden');
    };
};
