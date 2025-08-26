# Auth Polished SPA

Enterprise-grade React SPA to consume your .NET Auth API.

## Tech
- Vite + React + TS
- TailwindCSS (design tokens + shadcn-style UI components)
- React Router, React Query
- Axios with 401 refresh flow, JWT role parsing
- Sonner toasts, Lucide icons
- Protected routes + RequireRole guard

## Run
```bash
cp .env.example .env
npm i
npm run dev
```

Set `VITE_API_BASE` to your Auth API (default `http://localhost:5005`).

## Docker
```bash
docker build -t auth-polished-spa .
docker run -p 5173:5173 auth-polished-spa
```
