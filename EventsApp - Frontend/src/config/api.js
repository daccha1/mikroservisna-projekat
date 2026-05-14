const trimTrailingSlash = (value) => value.replace(/\/+$/, "");

const getBaseUrl = (envValue, fallback) => trimTrailingSlash(envValue || fallback);

export const API_SERVICES = {
  events: {
    baseUrl: getBaseUrl(import.meta.env.VITE_EVENTS_API_URL, "https://localhost:7042"),
  },
  locations: {
    baseUrl: getBaseUrl(import.meta.env.VITE_LOKACIJA_API_URL, "https://localhost:7042"),
  },
  organizers: {
    baseUrl: getBaseUrl(import.meta.env.VITE_ORGANIZATOR_API_URL, "https://localhost:7042"),
  },
  visitors: {
    baseUrl: getBaseUrl(import.meta.env.VITE_POSETILAC_API_URL, "http://localhost:5056"),
  },
};
