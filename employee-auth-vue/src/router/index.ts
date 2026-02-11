
import { createRouter, createWebHistory } from "vue-router";
import LoginView from "../views/LoginView.vue";
import ProfileView from "../views/ProfileView.vue";
import { getToken } from "../services/auth";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: "/", redirect: "/login" },
    { path: "/login", component: LoginView },
    { path: "/profile", component: ProfileView, meta: { requiresAuth: true } },
  ],
});

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !getToken()) return "/login";
  return true;
});

export default router;
