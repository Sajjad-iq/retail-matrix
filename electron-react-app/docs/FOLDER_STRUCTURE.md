# Project Folder Structure

This document describes the folder organization of this Electron + React application.

## 📁 Root Structure

```
electron-react-app/
├── app/                    # Renderer process (React application)
├── lib/                    # Main process code & shared utilities
├── resources/              # Static assets (icons, images)
├── electron.vite.config.ts # Vite configuration
├── package.json            # Dependencies & scripts
└── out/                    # Build output
```

---

## 📂 Detailed Structure

### `app/` - Renderer Process (React)
Contains the frontend React application code.

```
app/
├── components/             # Shared UI components
├── features/               # Feature modules (domain-driven)
├── hooks/                  # Shared custom hooks
├── stores/                 # Global state (Zustand)
├── styles/                 # Global styles (CSS/Tailwind)
├── app.tsx                 # Root component
├── routes.tsx              # Router configuration
└── renderer.tsx            # Entry point
```

**Purpose**: All UI and frontend logic goes here.

---

### `features/` - Feature Modules
Self-contained feature modules following domain-driven design.

```
features/
├── auth/
│   ├── components/        # Auth-specific components (LoginForm)
│   ├── hooks/             # Auth hooks (useAuthActions)
│   ├── lib/               # Types, utils
│   ├── pages/             # Page components (Login, Register)
│   └── services/          # API service calls
└── users/
    ├── components/
    ├── hooks/
    ├── lib/
    ├── pages/
    └── services/
```

**Purpose**: Each feature is independent and contains all related code.
**Rule**: Feature-specific code stays within the feature folder.

---

### `components/` - Shared Components
Reusable UI components used across multiple features.

```
components/
├── ui/                    # Shadcn UI primitives (Button, Input, etc.)
├── layouts/               # Layout wrappers (MainLayout, BlankLayout)
└── [category]/            # Other shared components
```

**Purpose**: Components shared across 2+ features.

---

### `lib/` - Main Process & Shared Config
Contains Electron main process code and shared configurations.

```
lib/
├── main/                  # Electron main process entry point
├── preload/               # Preload scripts (IPC bridges)
├── user-utils/            # Shared utilities
└── config/                # App configuration (HTTP, Query)
```

**Purpose**: Backend (Node.js) logic for Electron and shared config.

---

### `app/lib/config/http.ts` - HTTP Client & Error Handling
**Critical**: This file configures the Axios instance used by the application.

- **Centralized Error Handling**: A response interceptor catches all API errors.
- **Auto-Toast**: It automatically displays error messages using `sonner` toast.
- **Auth Interceptor**: Automatically attaches the Bearer token to requests.

---

## 🎯 Key Principles

### ✅ DO
- Keep feature-specific code in `features/[feature-name]/`
- Put shared components in `app/components/`
- Use `app/routes.tsx` for defining new routes
- rely on `http.ts` for error notifications (don't duplicate `toast.error` in hooks)

### ❌ DON'T
- duplicate error handling logic in individual hooks
- mix main process code (Node.js) with renderer code (React) outside of `lib/`
