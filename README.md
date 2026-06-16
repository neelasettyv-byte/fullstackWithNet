# Full-Stack Practice App — React + .NET Core with Login

An end-to-end app for practicing **how .NET deployment works** and **CI/CD pipelines that run on every push**.

## What's in here

```
fullstack-app/
├── api/                          # .NET 8 Web API (login + JWT auth)
│   ├── Program.cs                # all the API logic
│   └── api.csproj
├── api.tests/                    # xUnit tests for the API
├── frontend/                     # React app (login page)
│   └── src/App.jsx               # login UI that calls the API
├── azure-pipelines-api.yml       # pipeline for the API
└── azure-pipelines-frontend.yml  # pipeline for the frontend
```

## How the app works (the flow)

1. React shows a **login page**.
2. User enters username/password → React POSTs to the .NET API's `/api/login`.
3. The API checks credentials and returns a **JWT token** (a signed string proving who you are).
4. React stores the token. To access protected data, it calls `/api/secret` with the token attached.
5. The API verifies the token and returns data only if it's valid.

Test credentials: **admin / password123** (or quest / cricket).

---

## PART 1 — Run it locally first

You need **Node.js** and the **.NET 10 SDK** installed.

**Terminal 1 — start the API:**
```bash
cd api
dotnet run
```
It starts on something like `http://localhost:5000`. Test it: open `http://localhost:5000/api/health` — you should see a healthy status.

**Terminal 2 — start the frontend:**
```bash
cd frontend
npm install
npm run dev
```
Open the URL it prints. Log in with admin / password123, then click "Get protected data."

---

## PART 2 — How .NET deployment works (what you wanted to understand)

This is the key difference from the React-only app. A React app is **static files**. A .NET app is a **running program** — it must be compiled and executed on the server.

The .NET deploy has three phases, which you can see in `azure-pipelines-api.yml`:

1. **`dotnet restore`** — downloads the NuGet packages (dependencies).
2. **`dotnet build`** — compiles your C# code into runnable form.
3. **`dotnet publish`** — produces a self-contained folder with everything needed to run, which gets zipped and deployed.

On Azure, the API runs on an **App Service** configured for **.NET 8** (not static hosting like the React app). App Service runs your published DLL as a live process. This is why .NET *needs* a real App Service and can't use Static Web Apps the way the React frontend can.

### To deploy the API:

1. In Azure Portal, create an **App Service**: Runtime stack **.NET 10**, OS **Linux**, Free F1 tier, in your `rg-cicd-practice` group.
2. Note its name, put it in `azure-pipelines-api.yml` as `apiAppName`.
3. Set `azureSubscription` to your service connection name.

---

## PART 3 — Two pipelines, each runs on its own push

You asked for: **every push runs the pipeline.** Here it's smarter — each pipeline runs ONLY when its own folder changes, using **path filters**:

- Change anything in `api/` → the API pipeline runs (see `paths: include: api/*`).
- Change anything in `frontend/` → the frontend pipeline runs.

This is how real teams do it — you don't rebuild the whole world for a one-line frontend change.

### To set up each pipeline in Azure DevOps:

1. **Pipelines → New pipeline → GitHub → pick your repo.**
2. Choose **"Existing Azure Pipelines YAML file."**
3. Select `/azure-pipelines-api.yml` (do this once), then repeat for `/azure-pipelines-frontend.yml`.
4. Each becomes a separate pipeline that triggers on its own folder's changes.

---

## What to practice

- Push a change to `frontend/src/App.jsx` → watch ONLY the frontend pipeline run.
- Push a change to `api/Program.cs` → watch ONLY the API pipeline run.
- Break a test on purpose → watch the pipeline go red and block deployment.

That last one is the real lesson of CI/CD: broken code never reaches production.
