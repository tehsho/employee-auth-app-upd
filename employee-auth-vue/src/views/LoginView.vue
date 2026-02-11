<template>
  <v-container class="d-flex align-center justify-center" style="min-height: 100vh;">
    <v-card class="pa-6" max-width="420" width="100%">
      <v-card-title class="text-h6 text-center">
        Login
      </v-card-title>

      <v-card-text>
        <v-alert v-if="loginError" type="error" variant="tonal" class="mb-4">
          {{ loginError }}
        </v-alert>

        <v-form @submit.prevent="submitLogin">
          <v-text-field
            v-model="login"
            label="Username or Email"
            variant="outlined"
            class="mb-3"
          />

          <v-text-field
            v-model="password"
            label="Password"
            variant="outlined"
            :type="showPw ? 'text' : 'password'"
            :append-inner-icon="showPw ? 'mdi-eye-off' : 'mdi-eye'"
            @click:append-inner="showPw = !showPw"
            class="mb-4"
          />

          <v-btn type="submit" color="primary" block :loading="loading">
            Login
          </v-btn>

          <v-btn block variant="outlined" class="mt-3" @click="openRegister">
            Create an account
          </v-btn>
        </v-form>
      </v-card-text>
    </v-card>

    <!-- Register Modal -->
    <v-dialog v-model="showRegister" max-width="520">
      <v-card class="pa-4">
        <v-card-title class="d-flex align-center justify-space-between">
          <div class="text-h6">Create Account</div>
          <v-btn icon="mdi-close" variant="text" @click="closeRegister" />
        </v-card-title>

        <v-card-text>
          <v-alert v-if="regSuccess" type="success" variant="tonal" class="mb-3">
            {{ regSuccess }}
          </v-alert>
          <v-alert v-if="regError" type="error" variant="tonal" class="mb-3">
            {{ regError }}
          </v-alert>

          <v-form @submit.prevent="submitRegister">
            <v-text-field v-model="regUserName" label="Username" variant="outlined" class="mb-2" />
            <v-text-field v-model="regName" label="Full Name" variant="outlined" class="mb-2" />
            <v-text-field v-model="regEmail" label="Email" variant="outlined" class="mb-4" />

            <v-btn type="submit" color="success" block :loading="regLoading">
              Register
            </v-btn>

            <div class="text-caption text-medium-emphasis mt-3">
              After registration, the system generates a password and sends it by email.
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { saveToken } from "../services/auth";
import { apiFetch } from "../services/api";

const router = useRouter();

// login
const login = ref("");
const password = ref("");
const showPw = ref(false);
const loading = ref(false);
const loginError = ref("");

// register
const showRegister = ref(false);
const regUserName = ref("");
const regName = ref("");
const regEmail = ref("");
const regError = ref("");
const regSuccess = ref("");
const regLoading = ref(false);

async function submitLogin() {
  loginError.value = "";
  loading.value = true;

  try {
    const res = await apiFetch<{ token: string }>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ login: login.value, password: password.value }),
    });

    saveToken(res.token);
    router.push("/profile");
  } catch (e: any) {
    loginError.value = e?.message ?? "Login failed";
  } finally {
    loading.value = false;
  }
}

function openRegister() {
  showRegister.value = true;
  regError.value = "";
  regSuccess.value = "";
}

function closeRegister() {
  showRegister.value = false;
  regError.value = "";
  regSuccess.value = "";
}

async function submitRegister() {
  regError.value = "";
  regSuccess.value = "";
  regLoading.value = true;

  try {
    await apiFetch<{ id: string }>("/users", {
      method: "POST",
      body: JSON.stringify({
        userName: regUserName.value,
        name: regName.value,
        email: regEmail.value,
      }),
    });

    regSuccess.value = "Account created. Password was generated and sent by email.";

    regUserName.value = "";
    regName.value = "";
    regEmail.value = "";
  } catch (e: any) {
    regError.value = e?.message ?? "Registration failed";
  } finally {
    regLoading.value = false;
  }
}
</script>
