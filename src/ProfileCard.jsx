import { useState, useEffect } from "react";

function ProfileCard({ name, bio }) {
  const [likes, setLikes] = useState(0);
  const [showMore, setShowMore] = useState(false);
  const [status, setStatus] = useState("");

  useEffect(() => {
    document.title = `${name} - ${likes} likes`;
  }, [likes, name]);

  const skills = [
    { id: 1, name: "HTML" },
    { id: 2, name: "CSS" },
    { id: 3, name: "JavaScript" },
    { id: 4, name: "React" },
  ];

  return (
    <div className="card">
      <input
        type="text"
        value={status}
        onChange={(e) => setStatus(e.target.value)}
        placeholder="Enter your status"
      />

      <h2>{name}</h2>

      <p>{bio}</p>

      <p className="status">Status: {status}</p>

      {showMore && (
        <p>
          I enjoy building web applications, learning new technologies,
          and improving my programming skills every day.
        </p>
      )}

      <button onClick={() => setShowMore(!showMore)}>
        {showMore ? "Show less" : "Show more"}
      </button>

      <br />
      <br />

      <button onClick={() => setLikes(likes + 1)}>
        ♥ Like
      </button>

      <p className="likes">{likes} likes</p>

      <h3>Skills</h3>

      <ul>
        {skills.map((skill) => (
          <li key={skill.id}>{skill.name}</li>
        ))}
      </ul>
    </div>
  );
}

export default ProfileCard;