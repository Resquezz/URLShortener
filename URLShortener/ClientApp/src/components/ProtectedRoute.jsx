import React from "react";
import { Navigate } from "react-router-dom";

export default function ProtectedRoute({ auth, children }) {
    if (!auth) {
        return <Navigate to="/login" replace />;
    }

    return children;
}