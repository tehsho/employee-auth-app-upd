"use client";

import { useEffect, useState } from "react";
import { apiFetch } from "@/lib/api";
import { clearToken } from "@/lib/auth";
import { useRouter } from "next/navigation";
import { Eye, EyeOff } from "lucide-react";

type Me = {
  userName: string;
  name: string;
  email: string;
};

export default function ProfilePage() {
  const [me, setMe] = useState<Me | null>(null);

  const [name, setName] = useState("");

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const [showNew, setShowNew] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const router = useRouter();

  useEffect(() => {
    apiFetch<Me>("/users/me")
      .then((data) => {
        setMe(data);
        setName(data.name);
      })
      .catch(() => router.push("/login"));
  }, []);

  async function save() {
    setError("");
    setSuccess("");

    // Only validate passwords if user is trying to change password
    const wantsPasswordChange = newPassword.trim() || confirmPassword.trim();

    if (wantsPasswordChange) {
      if (!newPassword.trim()) return setError("Please enter a new password.");
      if (newPassword !== confirmPassword)
        return setError("New password and confirmation do not match.");
    }

    try {
      await apiFetch("/users/me", {
        method: "PUT",
        body: JSON.stringify({
          name,
          newPassword: wantsPasswordChange ? newPassword : null,
        }),
      });

      setSuccess("Updated!");
      setNewPassword("");
      setConfirmPassword("");
      setShowNew(false);
      setShowConfirm(false);

      setMe((prev) => (prev ? { ...prev, name } : prev));
    } catch (err: any) {
      const msg = err?.message || "Update failed";

      if (
        msg.toLowerCase().includes("unauthorized") ||
        msg.toLowerCase().includes("invalid credentials")
      ) {
        clearToken();
        router.push("/login");
        return;
      }

      setError(msg);
    }
  }

  function logout() {
    clearToken();
    router.push("/login");
  }

  if (!me) return null;

  return (
    <div className="flex min-h-screen justify-center bg-slate-100 p-8">
      <div className="w-[420px] space-y-4 rounded-xl bg-white p-6 shadow-md">
        <h1 className="text-xl font-bold">My Profile</h1>

        {error && <p className="text-sm text-red-600">{error}</p>}
        {success && <p className="text-sm text-green-600">{success}</p>}

        <div>
          <label className="text-sm text-gray-600">Username</label>
          <div className="rounded border bg-gray-50 p-2">{me.userName}</div>
        </div>

        <div>
          <label className="text-sm text-gray-600">Email</label>
          <div className="rounded border bg-gray-50 p-2">{me.email}</div>
        </div>

        <div>
          <label className="text-sm text-gray-600">Name</label>
          <input
            className="w-full rounded border p-2"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>

        <hr className="my-2" />

        {/* New Password */}
        <div>
          <label className="text-sm text-gray-600">New Password</label>
          <div className="relative">
            <input
              type={showNew ? "text" : "password"}
              className="w-full rounded border p-2 pr-10"
              value={newPassword}
              onChange={(e) => {
                setNewPassword(e.target.value);
                setShowNew(false); // auto-hide when typing
              }}
              placeholder="Enter new password"
            />
            <button
              type="button"
              onClick={() => setShowNew((v) => !v)}
              className="absolute inset-y-0 right-2 flex items-center text-gray-500 hover:text-gray-800"
              aria-label={showNew ? "Hide new password" : "Show new password"}
            >
              {showNew ? <EyeOff size={18} /> : <Eye size={18} />}
            </button>
          </div>
        </div>

        {/* Confirm Password */}
        <div>
          <label className="text-sm text-gray-600">Confirm New Password</label>
          <div className="relative">
            <input
              type={showConfirm ? "text" : "password"}
              className="w-full rounded border p-2 pr-10"
              value={confirmPassword}
              onChange={(e) => {
                setConfirmPassword(e.target.value);
                setShowConfirm(false); // auto-hide when typing
              }}
              placeholder="Confirm new password"
            />
            <button
              type="button"
              onClick={() => setShowConfirm((v) => !v)}
              className="absolute inset-y-0 right-2 flex items-center text-gray-500 hover:text-gray-800"
              aria-label={showConfirm ? "Hide confirm password" : "Show confirm password"}
            >
              {showConfirm ? <EyeOff size={18} /> : <Eye size={18} />}
            </button>
          </div>
        </div>

        <button
          onClick={save}
          className="w-full rounded bg-blue-600 p-2 text-white hover:bg-blue-700"
        >
          Save Changes
        </button>

        <button onClick={logout} className="w-full rounded border p-2">
          Logout
        </button>
      </div>
    </div>
  );
}
