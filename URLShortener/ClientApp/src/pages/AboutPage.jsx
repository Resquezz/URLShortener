import React, { useEffect, useState } from "react";
import { apiRequest } from "../api/apiClient";

export default function AboutPage({ auth }) {
    const isAdmin = auth?.role === "Admin";
    const [content, setContent] = useState("");
    const [draft, setDraft] = useState("");
    const [error, setError] = useState("");
    const [message, setMessage] = useState("");
    const [busy, setBusy] = useState(false);

    useEffect(() => {
        async function load() {
            try {
                const data = await apiRequest("/api/about");
                const value = data.content ?? "";
                setContent(value);
                setDraft(value);
            } catch (requestError) {
                setError(requestError.message);
            }
        }

        load();
    }, []);

    async function save() {
        setBusy(true);
        setError("");
        setMessage("");

        try {
            await apiRequest("/api/about", {
                method: "PUT",
                body: JSON.stringify(draft)
            }, auth.token);

            setContent(draft);
            setMessage("About content updated.");
        } catch (requestError) {
            setError(requestError.message);
        } finally {
            setBusy(false);
        }
    }

    return (
        <section className="container">
            <div className="card shadow-sm">
                <div className="card-body">
                    <h1 className="h4">About</h1>
                    <p className="text-muted">
                        This page describes the URL shortening algorithm and is visible for everyone.
                    </p>

                    {error ? <div className="alert alert-danger">{error}</div> : null}
                    {message ? <div className="alert alert-success">{message}</div> : null}

                    {isAdmin ? (
                        <div className="d-grid gap-2">
                            <textarea
                                className="form-control"
                                rows="10"
                                value={draft}
                                onChange={(event) => setDraft(event.target.value)}
                            ></textarea>
                            <div>
                                <button className="btn btn-primary" onClick={save} disabled={busy}>Save</button>
                            </div>
                        </div>
                    ) : (
                        <article className="border rounded p-3 bg-light">{content || "No description yet."}</article>
                    )}
                </div>
            </div>
        </section>
    );
}