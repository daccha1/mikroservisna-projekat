export default function Navbar({ onHome }) {
  return (
    <nav className="fixed top-0 left-0 right-0 z-50 bg-stone-950/80 backdrop-blur-md border-b border-stone-800">
      <div className="max-w-7xl mx-auto px-6 h-14 flex items-center justify-between">
        <button
          onClick={onHome}
          className="flex items-center gap-2 text-amber-400 font-bold text-lg tracking-tight hover:text-amber-300 transition-colors"
          style={{ fontFamily: "'Playfair Display', serif" }}
        >
          <span className="text-xl">◆</span>
          Event Management
        </button>
        <span className="text-stone-500 text-xs tracking-widest uppercase">Admin Panel</span>
      </div>
    </nav>
  );
}
