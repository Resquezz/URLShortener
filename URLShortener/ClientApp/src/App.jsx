import React, { useEffect, useMemo, useState } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import Header from "./components/Header";
import ProtectedRoute from "./components/ProtectedRoute";
import AuthPage from "./pages/AuthPage";
import UrlTablePage from "./pages/UrlTablePage";
import UrlInfoPage from "./pages/UrlInfoPage";
import AboutPage from "./pages/AboutPage";
import { decodeJwt } from "./utils/decodeJwt";

const TOKEN_KEY = "url-shortener-token";

export default function App() {
    const [token, setToken] = useState(() => localStorage.getItem(TOKEN_KEY));

    const auth = useMemo(() => {
        if (!token) {
            return null;
        }

        const claims = decodeJwt(token);
        if (!claims) {
            return null;
        }

        return {
            token,
            ...claims
        };
    }, [token]);

    useEffect(() => {
        if (token) {
            localStorage.setItem(TOKEN_KEY, token);
            return;
        }

        localStorage.removeItem(TOKEN_KEY);
    }, [token]);

    function handleLogin(newToken) {
        setToken(newToken);
    }

    function handleLogout() {
        setToken(null);
    }

    return (
        <div className="app-shell">
            <Header auth={auth} onLogout={handleLogout} />
            <main className="app-main">
                <Routes>
                    <Route path="/" element={<UrlTablePage auth={auth} />} />
                    <Route path="/login" element={<AuthPage auth={auth} onLogin={handleLogin} />} />
                    <Route
                        path="/urls/:id"
                        element={<ProtectedRoute auth={auth}><UrlInfoPage auth={auth} /></ProtectedRoute>}
                    />
                    <Route path="/about" element={<AboutPage auth={auth} />} />
                    <Route path="/register" element={<Navigate to="/login" replace />} />
                    <Route path="*" element={<Navigate to="/" replace />} />
                </Routes>
            </main>
        </div>
    );
}