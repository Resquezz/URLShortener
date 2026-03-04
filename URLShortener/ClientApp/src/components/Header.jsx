import React from "react";
import { NavLink } from "react-router-dom";

export default function Header({ auth, onLogout }) {
    return (
        <header className="app-header">
            <div className="container d-flex flex-wrap align-items-center justify-content-between py-3 gap-3">
                <NavLink to="/" className="brand">URL Shortener</NavLink>
                <nav className="d-flex flex-wrap gap-2">
                    <NavLink to="/" className="btn btn-outline-secondary btn-sm">Short URLs</NavLink>
                    <NavLink to="/about" className="btn btn-outline-secondary btn-sm">About</NavLink>
                    {!auth ? <NavLink to="/login" className="btn btn-outline-secondary btn-sm">Sign in</NavLink> : null}
                </nav>
                {auth ? (
                    <div className="d-flex align-items-center gap-2">
                        <span className="badge text-bg-light">{auth.username} ({auth.role})</span>
                        <button type="button" className="btn btn-sm btn-danger" onClick={onLogout}>Logout</button>
                    </div>
                ) : (
                    <span className="text-muted small">Anonymous mode</span>
                )}
            </div>
        </header>
    );
}