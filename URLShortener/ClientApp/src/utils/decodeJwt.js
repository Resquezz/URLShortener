export function decodeJwt(token) {
    try {
        const payload = token.split(".")[1];
        const normalized = payload.replace(/-/g, "+").replace(/_/g, "/");
        const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, "=");
        const json = decodeURIComponent(
            atob(padded)
                .split("")
                .map((char) => `%${(`00${char.charCodeAt(0).toString(16)}`).slice(-2)}`)
                .join("")
        );

        const parsed = JSON.parse(json);
        const expSeconds = Number(parsed.exp ?? 0);
        if (Number.isFinite(expSeconds) && expSeconds > 0) {
            const nowSeconds = Math.floor(Date.now() / 1000);
            if (expSeconds <= nowSeconds) {
                return null;
            }
        }

        return {
            username: parsed["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ?? "",
            userId: parsed["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ?? "",
            role: parsed["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ?? "User"
        };
    } catch {
        return null;
    }
}