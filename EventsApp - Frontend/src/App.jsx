import { useState } from "react";
import Navbar from "./components/Navbar";
import Home from "./pages/Home";
import EntityPage from "./pages/EntityPage";

const ENTITIES = [
  {
    key: "events",
    label: "Events",
    icon: "📅",
    route: "events",
    fields: {
      response: ["id", "naziv", "agenda", "datumVreme", "trajanje", "cena", "nazivLokacije", "nazivTipa", "nazivOrganizatora"],
      request: ["naziv", "agenda", "datumVreme", "trajanje", "cena", "lokacijaId", "tipId", "organizatorId"],
    },
    fieldLabels: {
      id: "ID", naziv: "Name", agenda: "Agenda", datumVreme: "Date & Time",
      trajanje: "Duration (min)", cena: "Price (€)", nazivLokacije: "Location",
      nazivTipa: "Type", nazivOrganizatora: "Organizer",
      lokacijaId: "Location ID", tipId: "Type ID", organizatorId: "Organizer ID",
    },
    fieldTypes: {
      datumVreme: "datetime-local", trajanje: "number", cena: "number",
      lokacijaId: "number", tipId: "number", organizatorId: "number",
    },
  },
  {
    key: "locations",
    label: "Locations",
    icon: "📍",
    route: "locations",
    fields: {
      response: ["id", "naziv", "adresa", "kapacitet", "spisakDogadjaja"],
      request: ["naziv", "adresa", "kapacitet"],
    },
    fieldLabels: {
      id: "ID", naziv: "Name", adresa: "Address", kapacitet: "Capacity",
      spisakDogadjaja: "Events",
    },
    fieldTypes: { kapacitet: "number" },
  },
  {
    key: "speakers",
    label: "Speakers",
    icon: "🎤",
    route: "speakers",
    fields: {
      response: ["id", "ime", "prezime", "titula", "oblastStrucnosti", "email", "listaDogadjaja"],
      request: ["ime", "prezime", "titula", "oblastStrucnosti", "email", "password"],
    },
    fieldLabels: {
      id: "ID", ime: "First Name", prezime: "Last Name", titula: "Title",
      oblastStrucnosti: "Expertise", email: "Email", password: "Password",
      listaDogadjaja: "Events",
    },
    fieldTypes: { email: "email", password: "password" },
  },
  {
    key: "organizers",
    label: "Organizers",
    icon: "🏢",
    route: "organizers",
    fields: {
      response: ["id", "ime", "prezime", "listaDogadjaja"],
      request: ["ime", "prezime"],
    },
    fieldLabels: {
      id: "ID", ime: "First Name", prezime: "Last Name", listaDogadjaja: "Events",
    },
    fieldTypes: {},
  },
  {
    key: "event-types",
    label: "Event Types",
    icon: "🏷️",
    route: "event-types",
    fields: {
      response: ["id", "naziv", "listaDogadjaja"],
      request: ["naziv"],
    },
    fieldLabels: { id: "ID", naziv: "Name", listaDogadjaja: "Events" },
    fieldTypes: {},
  },
  {
    key: "event-speakers",
    label: "Event–Speaker Links",
    icon: "🔗",
    route: "event-speakers",
    fields: {
      response: ["id", "rasporedPredavanja", "imePrezimePredavaca", "nazivDogadjaja"],
      request: ["rasporedPredavanja", "predavacId", "strucniDogadjajId"],
    },
    fieldLabels: {
      id: "ID", rasporedPredavanja: "Schedule", imePrezimePredavaca: "Speaker",
      nazivDogadjaja: "Event", predavacId: "Speaker ID", strucniDogadjajId: "Event ID",
    },
    fieldTypes: { rasporedPredavanja: "datetime-local", predavacId: "number", strucniDogadjajId: "number" },
  },
];

export default function App() {
  const [currentEntity, setCurrentEntity] = useState(null);

  return (
    <div className="min-h-screen bg-stone-950 text-stone-100 font-sans">
      <Navbar onHome={() => setCurrentEntity(null)} />
      {currentEntity === null ? (
        <Home entities={ENTITIES} onSelect={setCurrentEntity} />
      ) : (
        <EntityPage entity={currentEntity} onBack={() => setCurrentEntity(null)} />
      )}
    </div>
  );
}
