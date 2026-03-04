import React, { useEffect, useState } from "react";
import { NavLink } from "react-router-dom";
import { apiRequest, normalizeUrlRecord } from "../api/apiClient";

export default function UrlTablePage({ auth }) {
    const [records, setRecords] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [message, setMessage] = useState("");
    const [newUrl, setNewUrl] = useState("");
    const [busy, setBusy] = useState(false);

    const canAdd = Boolean(auth);

    async function loadRecords() {
        setError("");
        try {
            const data = await apiRequest("/api/urlshortener");
            setRecords(data.map(normalizeUrlRecord));
        } catch (requestError) {
            setError(requestError.message);
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        loadRecords();
    }, []);

    async function addRecord(event) {
        event.preventDefault();
        if (!newUrl.trim()) {
            return;
        }

        setBusy(true);
        setError("");
        setMessage("");
        try {
            const code = await apiRequest(`/api/urlshortener?longUrl=${encodeURIComponent(newUrl.trim())}`, {
                method: "POST"
            }, auth.token);

            setMessage(`URL shortened successfully. Code: ${code}`);
            setNewUrl("");
            await loadRecords();
        } catch (requestError) {
            setError(requestError.message);
        } finally {
            setBusy(false);
        }
    }

    async function deleteRecord(id) {
        setBusy(true);
        setError("");
        setMessage("");

        try {
            await apiRequest(`/api/urlshortener/${id}`, { method: "DELETE" }, auth.token);
            setRecords((prev) => prev.filter((item) => item.id !== id));
            setMessage("Record deleted.");
        } catch (requestError) {
            setError(requestError.message);
        } finally {
            setBusy(false);
        }
    }

    function canDelete(record) {
        if (!auth) {
            return false;
        }

        if (auth.role === "Admin") {
            return true;
        }

        return auth.userId === record.createdById;
    }

    return (
        <section className="container">
            <div className="card shadow-sm mb-3">
                <div className="card-body">
                    <div className="card-title-row">
                        <h1 className="h4 mb-0">Short URLs</h1>
                        <span className="text-muted small">Visible for everyone</span>
                    </div>
                    <div className="message-box mt-2">
                        {error ? <div className="alert alert-danger py-2 mb-0">{error}</div> : null}
                        {message ? <div className="alert alert-success py-2 mb-0">{message}</div> : null}
                    </div>
                </div>
            </div>

            {canAdd ? (
                <div className="card shadow-sm mb-3">
                    <div className="card-body">
                        <h2 className="h5">Add new Url</h2>
                        <form className="row g-2" onSubmit={addRecord}>
                            <div className="col-12 col-md-10">
                                <input
                                    className="form-control"
                                    placeholder="https://example.com/page"
                                    value={newUrl}
                                    onChange={(event) => setNewUrl(event.target.value)}
                                    required
                                />
                            </div>
                            <div className="col-12 col-md-2 d-grid">
                                <button className="btn btn-primary" disabled={busy}>Add</button>
                            </div>
                        </form>
                    </div>
                </div>
            ) : (
                <div className="alert alert-light border mb-3">
                    Log in to add and delete URLs.
                </div>
            )}

            <div className="card shadow-sm">
                <div className="card-body table-wrap">
                    {loading ? (
                        <p className="mb-0">Loading...</p>
                    ) : (
                        <table className="table align-middle">
                            <thead>
                                <tr>
                                    <th>Long Url</th>
                                    <th>Short Url</th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {records.map((record) => (
                                    <tr key={record.id}>
                                        <td>
                                            <div className="long-url" title={record.longUrl}>{record.longUrl}</div>
                                        </td>
                                        <td>
                                            <a className="short-link" target="_blank" href={`/r/${record.shortCode}`} rel="noreferrer">{`/r/${record.shortCode}`}</a>
                                        </td>
                                        <td className="d-flex flex-wrap gap-2">
                                            {auth ? <NavLink className="btn btn-sm btn-outline-primary" to={`/urls/${record.id}`}>Info</NavLink> : <span className="text-muted small">Login for info</span>}
                                            {canDelete(record) ? (
                                                <button
                                                    className="btn btn-sm btn-outline-danger"
                                                    onClick={() => deleteRecord(record.id)}
                                                    disabled={busy}
                                                >
                                                    Delete
                                                </button>
                                            ) : null}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    )}
                </div>
            </div>
        </section>
    );
}