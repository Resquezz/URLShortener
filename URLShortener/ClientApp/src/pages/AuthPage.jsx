import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { apiRequest } from "../api/apiClient";

export default function AuthPage({ auth, onLogin }) {
    const navigate = useNavigate();
    const [mode, setMode] = useState("login");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");
    const [message, setMessage] = useState("");
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (auth) {
            navigate("/", { replace: true });
        }
    }, [auth, navigate]);

    async function submit(event) {
        event.preventDefault();
        setError("");
        setMessage("");

        if (mode === "register" && password !== confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        setLoading(true);

        try {
            if (mode === "login") {
                const result = await apiRequest("/api/users/login", {
                    method: "POST",
                    body: JSON.stringify({ username: username.trim(), password })
                });

                onLogin(result.token);
                navigate("/", { replace: true });
            } else {
                await apiRequest("/api/users/register", {
                    method: "POST",
                    body: JSON.stringify({ username: username.trim(), password })
                });

                setMessage("Registration successful. Please sign in.");
                setMode("login");
                setPassword("");
                setConfirmPassword("");
            }
        } catch (requestError) {
            setError(requestError.message);
        } finally {
            setLoading(false);
        }
    }

    return (
        <section className="container">
            <div className="row justify-content-center">
                <div className="col-12 col-md-6 col-lg-5">
                    <div className="card shadow-sm">
                        <div className="card-body">
                            <div className="d-flex gap-2 mb-3">
                                <button
                                    type="button"
                                    className={`btn btn-sm ${mode === "login" ? "btn-primary" : "btn-outline-primary"}`}
                                    onClick={() => {
                                        setMode("login");
                                        setError("");
                                        setMessage("");
                                    }}
                                >
                                    Login
                                </button>
                                <button
                                    type="button"
                                    className={`btn btn-sm ${mode === "register" ? "btn-primary" : "btn-outline-primary"}`}
                                    onClick={() => {
                                        setMode("register");
                                        setError("");
                                        setMessage("");
                                    }}
                                >
                                    Register
                                </button>
                            </div>

                            <h1 className="h4 mb-3">{mode === "login" ? "Sign in" : "Create account"}</h1>
                            <form onSubmit={submit} className="d-grid gap-3">
                                <div>
                                    <label className="form-label">Login</label>
                                    <input
                                        className="form-control"
                                        value={username}
                                        onChange={(event) => setUsername(event.target.value)}
                                        minLength={3}
                                        maxLength={20}
                                        required
                                    />
                                </div>
                                <div>
                                    <label className="form-label">Password</label>
                                    <input type="password" className="form-control" value={password} onChange={(event) => setPassword(event.target.value)} required />
                                </div>
                                {mode === "register" ? (
                                    <div>
                                        <label className="form-label">Confirm password</label>
                                        <input
                                            type="password"
                                            className="form-control"
                                            value={confirmPassword}
                                            onChange={(event) => setConfirmPassword(event.target.value)}
                                            required
                                        />
                                    </div>
                                ) : null}
                                {error ? <div className="alert alert-danger py-2">{error}</div> : null}
                                {message ? <div className="alert alert-success py-2">{message}</div> : null}
                                <button className="btn btn-primary" disabled={loading}>
                                    {loading
                                        ? mode === "login" ? "Authorizing..." : "Registering..."
                                        : mode === "login" ? "Authorize" : "Create account"}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    );
}