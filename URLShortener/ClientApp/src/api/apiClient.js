export function normalizeUrlRecord(record) {
    return {
        id: record.id,
        longUrl: record.longURL ?? record.longUrl,
        shortCode: record.shortCode,
        createdAt: record.createdAt,
        createdById: record.createdById,
        createdByUsername: record.createdByUsername
    };
}

export async function apiRequest(path, options = {}, token = null) {
    const headers = new Headers(options.headers ?? {});
    if (!headers.has("Content-Type") && options.body !== undefined) {
        headers.set("Content-Type", "application/json");
    }
    if (token) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    const response = await fetch(path, {
        ...options,
        headers
    });

    if (!response.ok) {
        if (response.status === 401) {
            throw new Error("Session expired. Please sign in again.");
        }

        let message = "Request failed.";
        try {
            const payload = await response.json();
            const validationErrors = payload.errors && typeof payload.errors === "object"
                ? Object.values(payload.errors).flat().filter(Boolean)
                : [];

            message = payload.message
                ?? payload.Message
                ?? payload.title
                ?? payload.detail
                ?? validationErrors[0]
                ?? message;
        } catch {
            message = response.statusText || message;
        }

        throw new Error(message);
    }

    if (response.status === 204) {
        return null;
    }

    const contentType = response.headers.get("content-type") ?? "";
    if (contentType.includes("application/json")) {
        return response.json();
    }

    const text = await response.text();
    if (!text) {
        return null;
    }

    try {
        return JSON.parse(text);
    } catch {
        return text;
    }
}