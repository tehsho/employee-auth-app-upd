import "vuetify/styles/main.css";
import { createVuetify } from "vuetify";
import { md3 } from "vuetify/blueprints";
import "@mdi/font/css/materialdesignicons.css";

export default createVuetify({
  blueprint: md3,
  theme: {
    defaultTheme: "light",
  },
});
