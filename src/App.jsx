import "./App.css";
import ProfileCard from "./ProfileCard";

function App() {
  return (
    <div>
      <h1 style={{ textAlign: "center" }}>My Mini Profile</h1>

      <ProfileCard
        name="Arth Sharma"
        bio="Computer Science student learning React."
      />
    </div>
  );
}

export default App;