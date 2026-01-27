import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import LoginPage from "./page";

jest.mock("next/navigation", () => ({
  useRouter: () => ({ push: jest.fn() }),
}));

jest.mock("@/lib/api", () => ({
  apiFetch: jest.fn(),
}));

jest.mock("@/lib/auth", () => ({
  saveToken: jest.fn(),
}));

import { apiFetch } from "@/lib/api";
import { saveToken } from "@/lib/auth";

describe("LoginPage", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("opens and closes the registration modal", async () => {
    render(<LoginPage />);
    const user = userEvent.setup();

    await user.click(screen.getByRole("button", { name: /create an account/i }));
    expect(screen.getByText(/create account/i)).toBeInTheDocument();

    await user.click(screen.getByLabelText(/close/i));
    expect(screen.queryByText(/create account/i)).not.toBeInTheDocument();
  });

  test("submits login and saves token", async () => {
    (apiFetch as jest.Mock).mockResolvedValueOnce({ token: "abc" });

    render(<LoginPage />);
    const user = userEvent.setup();

    await user.type(screen.getByPlaceholderText(/username or email/i), "john");
    await user.type(screen.getByPlaceholderText(/password/i), "pw");

    await user.click(screen.getByRole("button", { name: /^login$/i }));

    expect(apiFetch).toHaveBeenCalledWith(
      "/auth/login",
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify({ login: "john", password: "pw" }),
      }),
    );

    expect(saveToken).toHaveBeenCalledWith("abc");
  });

  test("shows error if login fails", async () => {
    (apiFetch as jest.Mock).mockRejectedValueOnce(new Error("Invalid credentials."));

    render(<LoginPage />);
    const user = userEvent.setup();

    await user.type(screen.getByPlaceholderText(/username or email/i), "john");
    await user.type(screen.getByPlaceholderText(/password/i), "bad");

    await user.click(screen.getByRole("button", { name: /^login$/i }));

    expect(await screen.findByText(/invalid credentials/i)).toBeInTheDocument();
  });
});
