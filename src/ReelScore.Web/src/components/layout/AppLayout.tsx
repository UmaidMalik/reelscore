import { Film } from "lucide-react";
import { NavLink, Outlet } from "react-router";

const navigation = [
  {
    label: "Discover",
    to: "/discover",
  },
  {
    label: "Library",
    to: "/library",
  },
];

export function AppLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="sticky top-0 z-50 border-b bg-background/95 backdrop-blur">
        <div className="mx-auto flex h-16 w-full max-w-7xl items-center gap-8 px-4 sm:px-6 lg:px-8">
          <NavLink
            to="/discover"
            className="flex items-center gap-2 font-semibold"
          >
            <Film className="size-5 text-orange-500" />
            <span>ReelScore</span>
          </NavLink>

          <nav className="flex items-center gap-1">
            {navigation.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  [
                    "rounded-md px-3 py-2 text-sm transition-colors",
                    isActive
                      ? "bg-orange-500/10 text-orange-500"
                      : "text-muted-foreground hover:text-foreground",
                  ].join(" ")
                }
              >
                {item.label}
              </NavLink>
            ))}
          </nav>
        </div>
      </header>

      <main className="mx-auto w-full max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <Outlet />
      </main>
    </div>
  );
}