# Event Management UI

React + Tailwind CSS frontend za Event Management API.

## Setup

```bash
npm install
npm run dev
```

Otvori http://localhost:5173

## Pretpostavke

- Backend radi na `http://localhost:5062`
- Vite proxy preusmerjava sve API pozive automatski (nema CORS problema)
- Ako backend radi na drugom portu, izmeni `vite.config.js` i `BASE_URL` u `src/pages/EntityPage.jsx`

## Entiteti

| Label               | Route            | Opis                        |
|---------------------|------------------|-----------------------------|
| Events              | /events          | Stručni događaji             |
| Locations           | /locations       | Lokacije                    |
| Speakers            | /speakers        | Predavači                   |
| Organizers          | /organizers      | Organizatori                |
| Event Types         | /event-types     | Tipovi događaja             |
| Event–Speaker Links | /event-speakers  | Veza događaj–predavač       |

## Funkcionalnosti

- **GetAll** – automatski se učitava pri odabiru entiteta
- **GetById** – search polje u toolbar-u
- **POST** – dugme "+ Add New", otvara modal sa formom
- **PUT** – dugme "✎ Update by ID", unos ID-a + novih vrednosti
- **Refresh** – ↺ dugme za ponovni GetAll
