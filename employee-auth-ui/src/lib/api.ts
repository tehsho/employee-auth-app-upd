const API_BASE = "https://localhost:5001/api"; // adjust if needed

export async function apiFetch<T = any>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const token = typeof window !== "undefined" ? localStorage.getItem("token") : null;

  const headers: HeadersInit = {
    ...(options.body ? { "Content-Type": "application/json" } : {}),
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };

  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers,
  });

  // --- Handle 401 explicitly
  if (res.status === 401) {
    throw new Error("Unauthorized");
  }

  // --- If no content, don't try to parse JSON
  if (res.status === 204) {
    return undefined as T;
  }

  const text = await res.text();

  // Still no body (some APIs return 200 with empty body)
  if (!text) {
    if (!res.ok) throw new Error("Request failed");
    return undefined as T;
  }

  // Try parse JSON, otherwise return text
  let data: any;
  try {
    data = JSON.parse(text);
  } catch {
    data = text;
  }

  if (!res.ok) {
    // backend might return { error: "..." }
    if (data && typeof data === "object" && data.error) {
      throw new Error(data.error);
    }
    throw new Error(typeof data === "string" ? data : "Request failed");
  }

  return data as T;
}
