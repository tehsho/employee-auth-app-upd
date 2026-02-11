
<template>
  <v-container class="py-8 d-flex justify-center">
    <v-card class="pa-6" max-width="520" width="100%">
      <v-card-title class="text-h6">
        Employee Authentication & Profile Service
      </v-card-title>

      <v-card-subtitle class="mb-4">My Profile</v-card-subtitle>

      <v-alert v-if="error" type="error" variant="tonal" class="mb-3">{{ error }}</v-alert>
      <v-alert v-if="success" type="success" variant="tonal" class="mb-3">{{ success }}</v-alert>

      <div v-if="loading" class="text-medium-emphasis">Loading...</div>

      <div v-else>
        <v-text-field label="Username" variant="outlined" :model-value="me?.userName" readonly class="mb-2" />
        <v-text-field label="Email" variant="outlined" :model-value="me?.email" readonly class="mb-4" />

        <v-text-field v-model="name" label="Name" variant="outlined" class="mb-4" />

        <v-divider class="my-4" />

        <v-text-field
          v-model="newPassword"
          label="New Password"
          variant="outlined"
          :type="showNew ? 'text' : 'password'"
          :append-inner-icon="showNew ? 'mdi-eye-off' : 'mdi-eye'"
          @click:append-inner="showNew = !showNew"
          class="mb-2"
        />

        <v-text-field
          v-model="confirmPassword"
          label="Confirm New Password"
          variant="outlined"
          :type="showConfirm ? 'text' : 'password'"
          :append-inner-icon="showConfirm ? 'mdi-eye-off' : 'mdi-eye'"
          @click:append-inner="showConfirm = !showConfirm"
          class="mb-4"
        />

        <v-btn color="primary" block :loading="saving" @click="save">
          Save Changes
        </v-btn>

        <v-btn block variant="outlined" class="mt-3" @click="logout">
          Logout
        </v-btn>
      </div>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { apiFetch } from "../services/api";
import { clearToken } from "../services/auth";

type Me = { userName: string; name: string; email: string };

const router = useRouter();

const me = ref<Me | null>(null);
const name = ref("");

const newPassword = ref("");
const confirmPassword = ref("");
const showNew = ref(false);
const showConfirm = ref(false);

const loading = ref(true);
const saving = ref(false);
const error = ref("");
const success = ref("");

onMounted(async () => {
  await loadMe();
});

async function loadMe() {
  loading.value = true;
  error.value = "";
  success.value = "";

  try {
    const data = await apiFetch<Me>("/users/me");
    me.value = data;
    name.value = data.name;
  } catch (e: any) {
    clearToken();
    router.push("/login");
  } finally {
    loading.value = false;
  }
}

async function save() {
  error.value = "";
  success.value = "";
  saving.value = true;

  try {
    const wantsPw = newPassword.value.trim() || confirmPassword.value.trim();

    if (wantsPw) {
      if (!newPassword.value.trim()) throw new Error("Please enter a new password.");
      if (newPassword.value !== confirmPassword.value) throw new Error("New password and confirmation do not match.");
    }

    await apiFetch<void>("/users/me", {
      method: "PUT",
      body: JSON.stringify({
        name: name.value,
        newPassword: wantsPw ? newPassword.value : null,
      }),
    });

    success.value = "Updated!";
    newPassword.value = "";
    confirmPassword.value = "";
    showNew.value = false;
    showConfirm.value = false;

    if (me.value) me.value = { ...me.value, name: name.value };
  } catch (e: any) {
    error.value = e?.message ?? "Update failed";
    if (error.value.toLowerCase().includes("unauthorized")) {
      clearToken();
      router.push("/login");
    }
  } finally {
    saving.value = false;
  }
}

function logout() {
  clearToken();
  router.push("/login");
}
</script>
