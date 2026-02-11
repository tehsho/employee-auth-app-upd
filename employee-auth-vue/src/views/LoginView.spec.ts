import { describe, it, expect, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, h } from "vue";

const pushMock = vi.fn();

vi.mock("vue-router", () => ({
  useRouter: () => ({ push: pushMock }),
}));

vi.mock("../services/auth", () => ({
  saveToken: vi.fn(),
}));

vi.mock("../services/api", () => ({
  apiFetch: vi.fn(),
}));

import LoginView from "./LoginView.vue";
import { saveToken } from "../services/auth";
import { apiFetch } from "../services/api";

const SlotStub = defineComponent({
  setup(_, { slots }) {
    return () => h("div", slots.default?.());
  },
});

const VFormStub = defineComponent({
  emits: ["submit"],
  setup(_, { slots, emit }) {
    return () =>
      h(
        "form",
        {
          onSubmit: (e: Event) => {
            emit("submit", e);
          },
        },
        slots.default?.()
      );
  },
});

describe("LoginView", () => {
  it("submit calls API, saves token, navigates", async () => {
    (apiFetch as any).mockResolvedValueOnce({ token: "abc123" });

    const wrapper = mount(LoginView, {
      global: {
        stubs: {
          "v-container": SlotStub,
          "v-card": SlotStub,
          "v-card-title": SlotStub,
          "v-card-text": SlotStub,
          "v-alert": SlotStub,
          "v-dialog": SlotStub,
          "v-text-field": SlotStub,
          "v-btn": SlotStub,
          "v-form": VFormStub,
        },
      },
    });

    await wrapper.get("form").trigger("submit");

    expect(apiFetch).toHaveBeenCalled();
    expect(saveToken).toHaveBeenCalledWith("abc123");
    expect(pushMock).toHaveBeenCalledWith("/profile");
  });
});
