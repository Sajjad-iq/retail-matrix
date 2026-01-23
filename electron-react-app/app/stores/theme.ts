import { create } from 'zustand';

type Theme = 'dark' | 'light' | 'system';
type SystemTheme = 'dark' | 'light';

interface ThemeState {
  // State
  theme: Theme;
  systemTheme: SystemTheme;

  // Actions
  initTheme: () => void;
  applyTheme: () => void;
  setTheme: (theme: Theme) => void;
  toggleTheme: () => void;
  getEffectiveTheme: () => 'dark' | 'light';
}

/**
 * Theme Store - Manages dark/light theme
 *
 * Features:
 * - Supports dark, light, and system preference
 * - Persists to localStorage
 * - Auto-applies theme to document
 * - Listens to system theme changes
 */
export const useThemeStore = create<ThemeState>((set, get) => ({
  // Initial state
  theme: 'dark',
  systemTheme: 'dark',

  // Initialize theme from localStorage and system preference
  initTheme: () => {
    // Get saved theme or default to dark
    const savedTheme = (localStorage.getItem('theme') as Theme) || 'dark';

    // Detect system theme
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    const systemTheme: SystemTheme = mediaQuery.matches ? 'dark' : 'light';

    // Listen for system theme changes
    mediaQuery.addEventListener('change', (e) => {
      const newSystemTheme: SystemTheme = e.matches ? 'dark' : 'light';
      set({ systemTheme: newSystemTheme });
      get().applyTheme();
    });

    set({ theme: savedTheme, systemTheme });
    get().applyTheme();
  },

  // Apply theme to document
  applyTheme: () => {
    const effectiveTheme = get().getEffectiveTheme();
    const root = document.documentElement;

    if (effectiveTheme === 'dark') {
      root.classList.add('dark');
    } else {
      root.classList.remove('dark');
    }
  },

  // Set theme and persist to localStorage
  setTheme: (theme) => {
    set({ theme });
    localStorage.setItem('theme', theme);
    get().applyTheme();
  },

  // Toggle between light and dark (ignoring system)
  toggleTheme: () => {
    const currentEffectiveTheme = get().getEffectiveTheme();
    const newTheme: Theme = currentEffectiveTheme === 'dark' ? 'light' : 'dark';
    get().setTheme(newTheme);
  },

  // Get effective theme (resolves 'system' to actual theme)
  getEffectiveTheme: () => {
    const { theme, systemTheme } = get();
    return theme === 'system' ? systemTheme : theme;
  },
}));
