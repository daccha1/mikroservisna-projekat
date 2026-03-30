import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      "/events": "http://localhost:5062",
      "/locations": "http://localhost:5062",
      "/speakers": "http://localhost:5062",
      "/organizers": "http://localhost:5062",
      "/event-types": "http://localhost:5062",
      "/event-speakers": "http://localhost:5062",
    },
  },
});
