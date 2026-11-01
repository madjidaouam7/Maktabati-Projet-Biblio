window.biblioAuthCookie = {
    setToken: (name, token, expiresAt) => {
        const secure = window.location.protocol === "https:" ? "; Secure" : "";
        document.cookie = `${name}=${token}; expires=${new Date(expiresAt).toUTCString()}; path=/; SameSite=Lax${secure}`;
    },
    clearToken: (name) => {
        const secure = window.location.protocol === "https:" ? "; Secure" : "";
        document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; SameSite=Lax${secure}`;
    }
};
