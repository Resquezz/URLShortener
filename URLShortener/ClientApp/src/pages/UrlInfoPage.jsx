import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { apiRequest, normalizeUrlRecord } from "../api/apiClient";

export default function UrlInfoPage({ auth }) {
    const { id } = useParams();
    const navigate = useNavigate();
    const [item, setItem] = useState(null);
    const [error, setError] = useState("");

    useEffect(() => {
        async function load() {
            try {
                const result = await apiRequest(`/api/urlshortener/${id}`, {}, auth.token);
                setItem(normalizeUrlRecord(result));
            } catch (requestError) {
                setError(requestError.message);
            }
        }

        load();
    }, [auth.token, id]);

    return (
        <section className="container">
            <div className="card shadow-sm">
                <div className="card-body">
                    <div className="d-flex justify-content-between align-items-center mb-3">
                        <h1 className="h4 mb-0">Short URL Info</h1>
                        <button className="btn btn-outline-secondary btn-sm" onClick={() => navigate(-1)}>Back</button>
                    </div>

                    {error ? (
                        <div className="alert alert-danger">{error}</div>
                    ) : item ? (
                        <div className="info-grid">
                            <div className="info-key">Id</div><div>{item.id}</div>
                            <div className="info-key">Long URL</div><div>{item.longUrl}</div>
                            <div className="info-key">Short URL</div><div><a href={`/r/${item.shortCode}`} target="_blank" rel="noreferrer">{`/r/${item.shortCode}`}</a></div>
                            <div className="info-key">Created Date</div><div>{new Date(item.createdAt).toLocaleString()}</div>
                            <div className="info-key">Created By</div><div>{item.createdByUsername ?? item.createdById ?? "-"}</div>
                        </div>
                    ) : (
                        <p>Loading...</p>
                    )}
                </div>
            </div>
        </section>
    );
}