"use client";

import { useState } from "react";
import { apiFetch } from "@/lib/api";
import { useRouter } from "next/navigation";

export default function RegisterPage() {
  const [userName, setUserName] = useState("");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");

  const [msg, setMsg] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const router = useRouter();

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setMsg("");
    setLoading(true);

    try {
      await apiFetch("/users", {
        method: "POST",
        body: JSON.stringify({ userName, name, email }),
      });

      setMsg(
        "Account created! Check your email for your password. Redirecting to login...",
      );

      setTimeout(() => {
        router.push("/login");
      }, 1500);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-100">
      <form
        onSubmit={submit}
        className="w-96 space-y-4 rounded-xl bg-white p-8 shadow-md"
      >
        <h1 className="text-center text-2xl font-bold">Create Account</h1>

        {msg && <p className="text-sm text-green-600">{msg}</p>}
        {error && <p className="text-sm text-red-600">{error}</p>}

        <input
          className="w-full rounded border p-2"
          placeholder="Username"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
          disabled={loading}
        />

        <input
          className="w-full rounded border p-2"
          placeholder="Name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={loading}
        />

        <input
          className="w-full rounded border p-2"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          disabled={loading}
        />

        <button
          type="submit"
          disabled={loading}
          className="w-full rounded bg-green-600 p-2 text-white hover:bg-green-700 disabled:opacity-60"
        >
          {loading ? "Creating..." : "Register"}
        </button>
      </form>
    </div>
  );
}
