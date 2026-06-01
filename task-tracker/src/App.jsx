import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Checkbox } from "@/components/ui/checkbox";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

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
  const [tasks, setTasks] = useState([]);
  const [input, setInput] = useState("");
  const [filter, setFilter] = useState("All");
  const [priority, setPriority] = useState("Medium");
  const [dueDate, setDueDate] = useState("");
  const [theme, setTheme] = useState("gray");

  const addTask = () => {
    if (!input.trim()) return;
    setTasks([
      ...tasks,
      {
        id: Date.now(),
        text: input.trim(),
        done: false,
        priority,
        dueDate,
      },
    ]);
    setInput("");
    setDueDate("");
    setPriority("Medium");
  };

  const toggleTask = (id) => {
    setTasks(tasks.map((t) => (t.id === id ? { ...t, done: !t.done } : t)));
  };

  const deleteTask = (id) => {
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

  return (
    <div className={`min-h-screen ${THEMES[theme]} flex items-center justify-center p-4 transition-colors duration-300`}>
      <Card className="w-full max-w-lg shadow-xl">
        <CardHeader>
          <CardTitle className="text-2xl font-bold text-center">
            Task Tracker
          </CardTitle>
          {/* Theme Switcher */}
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
          {/* Input Row */}
          <div className="flex gap-2">
            <Input
              placeholder="Add a new task..."
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && addTask()}
            />
            <Button onClick={addTask}>Add</Button>
          </div>

          {/* Priority + Due Date Row */}
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
            <Input
              type="date"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
              className="w-full"
            />
          </div>

          {/* Filters */}
          <div className="flex gap-2">
            {FILTERS.map((f) => (
              <Button
                key={f}
                variant={filter === f ? "default" : "outline"}
                size="sm"
                onClick={() => setFilter(f)}
              >
                {f}
              </Button>
            ))}
          </div>

          {/* Task List */}
          <div className="space-y-2">
            {filtered.length === 0 && (
              <p className="text-center text-gray-400 text-sm py-4">
                No tasks here!
              </p>
            )}
            {filtered.map((task) => (
              <div
                key={task.id}
                className={`flex items-center justify-between bg-white rounded-lg px-4 py-3 shadow-sm border-l-4 ${
                  task.priority === "High"
                    ? "border-red-400"
                    : task.priority === "Medium"
                    ? "border-yellow-400"
                    : "border-green-400"
                }`}
              >
                <div className="flex items-center gap-3">
                  <Checkbox
                    checked={task.done}
                    onCheckedChange={() => toggleTask(task.id)}
                  />
                  <div>
                    <p className={task.done ? "line-through text-gray-400" : ""}>
                      {task.text}
                    </p>
                    {task.dueDate && (
                      <p className={`text-xs mt-0.5 ${isOverdue(task.dueDate, task.done) ? "text-red-500 font-semibold" : "text-gray-400"}`}>
                        {isOverdue(task.dueDate, task.done) ? "Overdue: " : "Due: "}
                        {task.dueDate}
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
                  <Button
                    variant="ghost"
                    size="sm"
                    className="text-red-500 hover:text-red-700"
                    onClick={() => deleteTask(task.id)}
                  >
                    X
                  </Button>
                </div>
              </div>
            ))}
          </div>

          {/* Summary */}
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