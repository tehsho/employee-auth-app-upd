
import { clearToken, getToken } from "../services/auth";

const API_BASE =
  import.meta.env.VITE_API_BASE?.toString() ?? "https://localhost:5001/api";

export async function apiFetch<T>(
  path: string,
  init: RequestInit = {}
): Promise<T> {
  const headers = new Headers(init.headers);

  headers.set("Content-Type", "application/json");

  const token = getToken();
  if (token) headers.set("Authorization", `Bearer ${token}`);

  const res = await fetch(`${API_BASE}${path}`, { ...init, headers });

  if (res.status === 401) {
    clearToken();
    throw new Error("Unauthorized");
  }

  // Avoid 'Unexpected end of JSON input' when backend returns empty body
  const text = await res.text();
  const data = text ? JSON.parse(text) : null;

  if (!res.ok) {
    const msg = data?.error ?? data?.message ?? `HTTP ${res.status}`;
    throw new Error(msg);
  }

  return data as T;
}
