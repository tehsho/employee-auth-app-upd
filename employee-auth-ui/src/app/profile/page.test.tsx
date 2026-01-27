import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import ProfilePage from "./page";

jest.mock("next/navigation", () => ({
  useRouter: () => ({ push: jest.fn() }),
}));

jest.mock("@/lib/api", () => ({
  apiFetch: jest.fn(),
}));

jest.mock("@/lib/auth", () => ({
  clearToken: jest.fn(),
}));

import { apiFetch } from "@/lib/api";

describe("ProfilePage", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("loads and displays user info", async () => {
    (apiFetch as jest.Mock).mockResolvedValueOnce({
      userName: "john",
      name: "John Doe",
      email: "john@test.com",
    });

    render(<ProfilePage />);

    expect(await screen.findByText("john")).toBeInTheDocument();
    expect(screen.getByText("john@test.com")).toBeInTheDocument();
    expect(screen.getByDisplayValue("John Doe")).toBeInTheDocument();
  });

  test("shows error if new password and confirm do not match", async () => {
    // first apiFetch call is GET /users/me
    (apiFetch as jest.Mock).mockResolvedValueOnce({
      userName: "john",
      name: "John Doe",
      email: "john@test.com",
    });

    render(<ProfilePage />);
    const user = userEvent.setup();

    await screen.findByText(/my profile/i);

    await user.type(screen.getByPlaceholderText(/enter new password/i), "Test#123!");
    await user.type(
      screen.getByPlaceholderText(/confirm new password/i),
      "Different#123!",
    );

    await user.click(screen.getByRole("button", { name: /save changes/i }));

    expect(await screen.findByText(/do not match/i)).toBeInTheDocument();

    // should not attempt PUT when validation fails
    expect(apiFetch).toHaveBeenCalledTimes(1);
  });

  test("calls PUT /users/me when saving name only", async () => {
    (apiFetch as jest.Mock)
      .mockResolvedValueOnce({
        userName: "john",
        name: "John Doe",
        email: "john@test.com",
      })
      .mockResolvedValueOnce(undefined); // PUT result

    render(<ProfilePage />);
    const user = userEvent.setup();

    await screen.findByText(/my profile/i);

    const nameInput = screen.getByDisplayValue("John Doe");
    await user.clear(nameInput);
    await user.type(nameInput, "John Updated");

    await user.click(screen.getByRole("button", { name: /save changes/i }));

    await waitFor(() => {
      expect(apiFetch).toHaveBeenCalledWith(
        "/users/me",
        expect.objectContaining({
          method: "PUT",
        }),
      );
    });
  });
});
