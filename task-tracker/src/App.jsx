import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

const API = "http://localhost:8080";
const FILTERS = ["All", "Active", "Completed"];
const PRIORITIES = ["Low", "Medium", "High"];
const THEMES = {
  gray: "bg-gray-100",
  blue: "bg-blue-50",
  green: "bg-green-50",
  purple: "bg-purple-50",
};
const PRIORITY_COLORS = {
  High: "bg-red-100 text-red-700",
  Medium: "bg-yellow-100 text-yellow-700",
  Low: "bg-green-100 text-green-700",
};

export default function App() {
  const [user, setUser] = useState(null);
  const [authMode, setAuthMode] = useState("login");
  const [authForm, setAuthForm] = useState({ name: "", email: "", password: "" });
  const [authError, setAuthError] = useState("");
  const [tasks, setTasks] = useState([]);
  const [input, setInput] = useState("");
  const [filter, setFilter] = useState("All");
  const [priority, setPriority] = useState("Medium");
  const [dueDate, setDueDate] = useState("");
  const [theme, setTheme] = useState("gray");

  useEffect(() => {
    const saved = localStorage.getItem("user");
    if (saved) setUser(JSON.parse(saved));
  }, []);

  useEffect(() => {
    if (user) {
      fetch(`${API}/tasks`)
        .then((res) => res.json())
        .then((data) => setTasks(data));
    }
  }, [user]);

  const handleAuth = async () => {
    setAuthError("");
    const endpoint = authMode === "login" ? "/auth/login" : "/auth/register";
    const body = authMode === "login"
      ? { email: authForm.email, password: authForm.password }
      : { name: authForm.name, email: authForm.email, password: authForm.password };

    const res = await fetch(`${API}${endpoint}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });
    const data = await res.json();
    if (data.error) {
      setAuthError(data.error);
    } else {
      localStorage.setItem("user", JSON.stringify(data));
      setUser(data);
    }
  };

  const logout = () => {
    localStorage.removeItem("user");
    setUser(null);
    setTasks([]);
  };

  const deleteAccount = async () => {
    if (!window.confirm("Are you sure you want to delete your account?")) return;
    await fetch(`${API}/auth/delete-account`, {
      method: "DELETE",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: user.email }),
    });
    localStorage.removeItem("user");
    setUser(null);
    setTasks([]);
  };

  const addTask = async () => {
    if (!input.trim()) return;
    const newTask = { text: input.trim(), done: false, priority, dueDate: dueDate || null };
    const res = await fetch(`${API}/tasks`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(newTask),
    });
    const saved = await res.json();
    setTasks([...tasks, saved]);
    setInput("");
    setDueDate("");
    setPriority("Medium");
  };

  const toggleTask = async (task) => {
    const updated = { ...task, done: !task.done };
    const res = await fetch(`${API}/tasks/${task.id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(updated),
    });
    const saved = await res.json();
    setTasks(tasks.map((t) => (t.id === saved.id ? saved : t)));
  };

  const deleteTask = async (id) => {
    await fetch(`${API}/tasks/${id}`, { method: "DELETE" });
    setTasks(tasks.filter((t) => t.id !== id));
  };

  const isOverdue = (dueDate, done) => {
    if (!dueDate || done) return false;
    return new Date(dueDate) < new Date();
  };

  const filtered = tasks.filter((t) => {
    if (filter === "Active") return !t.done;
    if (filter === "Completed") return t.done;
    return true;
  });

  if (!user) {
    return (
      <div className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
        <Card className="w-full max-w-md shadow-xl">
          <CardHeader>
            <CardTitle className="text-2xl font-bold text-center">
              {authMode === "login" ? "Login" : "Create Account"}
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {authMode === "register" && (
              <Input
                placeholder="Full Name"
                value={authForm.name}
                onChange={(e) => setAuthForm({ ...authForm, name: e.target.value })}
              />
            )}
            <Input
              placeholder="Email"
              type="email"
              value={authForm.email}
              onChange={(e) => setAuthForm({ ...authForm, email: e.target.value })}
            />
            <Input
              placeholder="Password"
              type="password"
              value={authForm.password}
              onChange={(e) => setAuthForm({ ...authForm, password: e.target.value })}
              onKeyDown={(e) => e.key === "Enter" && handleAuth()}
            />
            {authError && (
              <p className="text-red-500 text-sm">{authError}</p>
            )}
            <Button className="w-full" onClick={handleAuth}>
              {authMode === "login" ? "Login" : "Register"}
            </Button>
            <p className="text-center text-sm text-gray-500">
              {authMode === "login" ? "No account?" : "Already have an account?"}{" "}
              <span
                className="text-blue-500 cursor-pointer hover:underline"
                onClick={() => {
                  setAuthMode(authMode === "login" ? "register" : "login");
                  setAuthError("");
                }}
              >
                {authMode === "login" ? "Register" : "Login"}
              </span>
            </p>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className={`min-h-screen ${THEMES[theme]} flex items-center justify-center p-4 transition-colors duration-300`}>
      <Card className="w-full max-w-lg shadow-xl">
        <CardHeader>
          <div className="flex justify-between items-center">
            <CardTitle className="text-2xl font-bold">Task Tracker</CardTitle>
            <div className="flex items-center gap-2">
              <span className="text-sm text-gray-500">Hi, {user.name}</span>
              <Button variant="outline" size="sm" onClick={logout}>Logout</Button>
              <Button variant="destructive" size="sm" onClick={deleteAccount}>Delete Account</Button>
            </div>
          </div>
          <div className="flex justify-center gap-2 pt-2">
            {Object.keys(THEMES).map((t) => (
              <button
                key={t}
                onClick={() => setTheme(t)}
                className={`w-6 h-6 rounded-full border-2 ${THEMES[t]} ${theme === t ? "border-black" : "border-gray-300"}`}
              />
            ))}
          </div>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex gap-2">
            <Input
              placeholder="Add a new task..."
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && addTask()}
            />
            <Button onClick={addTask}>Add</Button>
          </div>
          <div className="flex gap-2">
            <Select value={priority} onValueChange={setPriority}>
              <SelectTrigger className="w-[140px]">
                <SelectValue placeholder="Priority" />
              </SelectTrigger>
              <SelectContent>
                {PRIORITIES.map((p) => (
                  <SelectItem key={p} value={p}>{p}</SelectItem>
                ))}
              </SelectContent>
            </Select>
            <Input type="date" value={dueDate} onChange={(e) => setDueDate(e.target.value)} className="w-full" />
          </div>
          <div className="flex gap-2">
            {FILTERS.map((f) => (
              <Button key={f} variant={filter === f ? "default" : "outline"} size="sm" onClick={() => setFilter(f)}>
                {f}
              </Button>
            ))}
          </div>
          <div className="space-y-2">
            {filtered.length === 0 && (
              <p className="text-center text-gray-400 text-sm py-4">No tasks here!</p>
            )}
            {filtered.map((task) => (
              <div
                key={task.id}
                className={`flex items-center justify-between bg-white rounded-lg px-4 py-3 shadow-sm border-l-4 ${
                  task.priority === "High" ? "border-red-400" : task.priority === "Medium" ? "border-yellow-400" : "border-green-400"
                }`}
              >
                <div className="flex items-center gap-3">
                  <Checkbox checked={task.done} onCheckedChange={() => toggleTask(task)} />
                  <div>
                    <p className={task.done ? "line-through text-gray-400" : ""}>{task.text}</p>
                    {task.dueDate && (
                      <p className={`text-xs mt-0.5 ${isOverdue(task.dueDate, task.done) ? "text-red-500 font-semibold" : "text-gray-400"}`}>
                        {isOverdue(task.dueDate, task.done) ? "Overdue: " : "Due: "}{task.dueDate}
                      </p>
                    )}
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <span className={`text-xs px-2 py-1 rounded-full font-medium ${PRIORITY_COLORS[task.priority]}`}>
                    {task.priority}
                  </span>
                  <Badge variant={task.done ? "secondary" : "default"}>
                    {task.done ? "Done" : "Active"}
                  </Badge>
                  <Button variant="ghost" size="sm" className="text-red-500 hover:text-red-700" onClick={() => deleteTask(task.id)}>
                    X
                  </Button>
                </div>
              </div>
            ))}
          </div>
          {tasks.length > 0 && (
            <p className="text-sm text-gray-400 text-center">
              {tasks.filter((t) => t.done).length}/{tasks.length} tasks completed
            </p>
          )}
        </CardContent>
      </Card>
    </div>
  );
}