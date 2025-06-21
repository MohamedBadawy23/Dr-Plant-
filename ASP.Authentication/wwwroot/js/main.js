document.addEventListener('DOMContentLoaded', function() {
    // Feature card click handlers
    document.querySelectorAll('.feature-card').forEach(card => {
        card.addEventListener('click', function() {
            const route = this.getAttribute('onclick').split("'")[1];
            // Add animation before navigation
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = 'scale(1)';
                window.location.href = route;
            }, 200);
        });
    });

    // Chat bot toggle
    const chatBot = document.querySelector('.chat-bot');
    chatBot.addEventListener('click', function() {
        // Add chat bot functionality here
        alert('Chat bot feature coming soon!');
    });

    // Profile icon click handler
    const profileImage = document.getElementById('profileImage');
    profileImage.addEventListener('click', function() {
        window.location.href = '/profile';
    });

    // Info icon click handler
    const infoIcon = document.querySelector('.info-icon');
    infoIcon.addEventListener('click', function() {
        // Add info modal functionality here
        alert('App Information: Plant Disease Recognition System');
    });
}); 