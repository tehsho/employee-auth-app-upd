"use client";

import { useState } from "react";
import { apiFetch } from "@/lib/api";
import { saveToken } from "@/lib/auth";
import { useRouter } from "next/navigation";
import { Eye, EyeOff } from "lucide-react";

export default function LoginPage() {
  // Login form
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [loginError, setLoginError] = useState("");

  // Register modal
  const [showRegister, setShowRegister] = useState(false);
  const [regUserName, setRegUserName] = useState("");
  const [regName, setRegName] = useState("");
  const [regEmail, setRegEmail] = useState("");
  const [regError, setRegError] = useState("");
  const [regSuccess, setRegSuccess] = useState("");
  const [regLoading, setRegLoading] = useState(false);

  const router = useRouter();

  async function submitLogin(e: React.FormEvent) {
    e.preventDefault();
    setLoginError("");

    try {
      const res = await apiFetch<{ token: string }>("/auth/login", {
        method: "POST",
        body: JSON.stringify({ login, password }),
      });

      saveToken(res.token);
      router.push("/profile");
    } catch (err: any) {
      setLoginError(err.message);
    }
  }

  function openRegister() {
    setShowRegister(true);
    setRegError("");
    setRegSuccess("");
  }

  function closeRegister() {
    setShowRegister(false);
    setRegError("");
    setRegSuccess("");
  }

  async function submitRegister(e: React.FormEvent) {
    e.preventDefault();
    setRegError("");
    setRegSuccess("");
    setRegLoading(true);

    try {
      await apiFetch("/users", {
        method: "POST",
        body: JSON.stringify({
          userName: regUserName,
          name: regName,
          email: regEmail,
        }),
      });

      setRegSuccess("Account created ✅ Password was generated and sent by email.");

      setRegUserName("");
      setRegName("");
      setRegEmail("");
    } catch (err: any) {
      setRegError(err.message);
    } finally {
      setRegLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100">
      {/* LOGIN CARD (visible under popup) */}
      <form
        onSubmit={submitLogin}
        className="w-96 space-y-4 rounded-xl bg-white p-8 shadow-md"
      >
        <h1 className="text-center text-2xl font-bold">Login</h1>

        {loginError && <p className="text-sm text-red-600">{loginError}</p>}

        <input
          className="w-full rounded border p-2"
          placeholder="Username or Email"
          value={login}
          onChange={(e) => setLogin(e.target.value)}
        />

        {/* Password with eye toggle */}
        <div className="relative">
          <input
            type={showPassword ? "text" : "password"}
            className="w-full rounded border p-2 pr-10"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          <button
            type="button"
            onClick={() => setShowPassword((v) => !v)}
            className="absolute inset-y-0 right-2 flex items-center text-gray-500 hover:text-gray-800"
            aria-label="Toggle password visibility"
          >
            {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
          </button>
        </div>

        <button className="w-full rounded bg-blue-600 p-2 text-white hover:bg-blue-700">
          Login
        </button>

        <button
          type="button"
          onClick={openRegister}
          className="w-full rounded border p-2"
        >
          Create an account
        </button>
      </form>

      {/* REGISTER POPUP */}
      {showRegister && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-white/40 p-4 backdrop-blur-sm">
          <div className="relative w-full max-w-md space-y-4 rounded-xl bg-white p-6 shadow-lg">
            {/* Close icon */}
            <button
              type="button"
              onClick={closeRegister}
              className="absolute top-3 right-3 text-xl text-gray-400 hover:text-gray-700"
              aria-label="Close"
            >
              ✕
            </button>

            <h2 className="text-center text-xl font-bold">Create Account</h2>

            {regSuccess && (
              <div className="rounded border border-green-200 bg-green-50 p-2 text-sm text-green-700">
                {regSuccess}
              </div>
            )}

            {regError && (
              <div className="rounded border border-red-200 bg-red-50 p-2 text-sm text-red-700">
                {regError}
              </div>
            )}

            <form onSubmit={submitRegister} className="space-y-3">
              <input
                className="w-full rounded border p-2"
                placeholder="Username"
                value={regUserName}
                onChange={(e) => setRegUserName(e.target.value)}
                disabled={regLoading}
              />

              <input
                className="w-full rounded border p-2"
                placeholder="Full Name"
                value={regName}
                onChange={(e) => setRegName(e.target.value)}
                disabled={regLoading}
              />

              <input
                className="w-full rounded border p-2"
                placeholder="Email"
                value={regEmail}
                onChange={(e) => setRegEmail(e.target.value)}
                disabled={regLoading}
              />

              <button
                type="submit"
                disabled={regLoading}
                className="w-full rounded bg-green-600 p-2 text-white hover:bg-green-700 disabled:opacity-60"
              >
                {regLoading ? "Creating..." : "Register"}
              </button>
            </form>

            <p className="text-xs text-gray-500">
              After registration, the system generates a password and sends it by email.
            </p>
          </div>
        </div>
      )}
    </div>
  );
}
