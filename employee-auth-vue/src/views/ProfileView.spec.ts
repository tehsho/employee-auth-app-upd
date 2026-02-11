import { describe, it, expect, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, h } from "vue";

const pushMock = vi.fn();

vi.mock("vue-router", () => ({
  useRouter: () => ({ push: pushMock }),
}));

vi.mock("../services/auth", () => ({
  clearToken: vi.fn(),
}));

vi.mock("../services/api", () => ({
  apiFetch: vi.fn(),
}));

import ProfileView from "./ProfileView.vue";
import { clearToken } from "../services/auth";
import { apiFetch } from "../services/api";

const SlotStub = defineComponent({
  setup(_, { slots }) {
    return () => h("div", slots.default?.());
  },
});

describe("ProfileView", () => {
  it("if /users/me fails -> clears token and redirects to /login", async () => {
    (apiFetch as any).mockRejectedValueOnce(new Error("Unauthorized"));

    mount(ProfileView, {
      global: {
        stubs: {
          "v-container": SlotStub,
          "v-card": SlotStub,
          "v-card-title": SlotStub,
          "v-card-subtitle": SlotStub,
          "v-card-text": SlotStub,
          "v-alert": SlotStub,
          "v-text-field": SlotStub,
          "v-divider": SlotStub,
          "v-btn": SlotStub,
        },
      },
    });

    await Promise.resolve();

    expect(clearToken).toHaveBeenCalled();
    expect(pushMock).toHaveBeenCalledWith("/login");
  });
});
