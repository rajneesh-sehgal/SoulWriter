window.soulWriterAutoResize = (textarea) => {
    if (textarea) {
        textarea.style.height = 'auto';
        textarea.style.height = (textarea.scrollHeight) + 'px';
    }
};

window.scrollChatToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};