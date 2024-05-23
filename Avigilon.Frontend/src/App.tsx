import "./App.css";
import { TimelineRequestBody } from "../types/TimelineRequestBody";
import { CameraTimeline, Record } from "../types/Timeline";
import { useState } from "react";

function App() {
  const [inputRequestBody, setInputRequestBody] = useState<TimelineRequestBody>(
    {
      startDate: "",
      endDate: "",
      interval: "",
      camera: "",
    }
  );
  const [timeLines, setTimelines] = useState<CameraTimeline[] | null>(null);
  const [errorMessage, setErrorMessage] = useState();

  function handleChange(event: React.ChangeEvent<HTMLInputElement>) {
    const { name, value } = event.target;
    setInputRequestBody((prevState) => ({
      ...prevState,
      [name]: value,
    }));
  }

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    console.log(inputRequestBody);
    getTimeline();
  }

  function getTimeline() {
    const requestOptions = {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(inputRequestBody),
    };
    const fetchData = async () => {
      const response = await fetch(
        "https://localhost:44390/getList",
        requestOptions
      );

      if (!response.ok) {
        const errorFromFetch = await response.json();
        setErrorMessage(errorFromFetch);
      } else {
        const data = (await response.json()) as CameraTimeline[];
        console.log(data);
        setTimelines(data);
      }
    };
    fetchData();
    console.log("Running get Timeline api call");
    console.log(timeLines);
  }

  return (
    <>
      <form onSubmit={handleSubmit}>
        <div>
          <input
            type="text"
            name="startDate"
            value={inputRequestBody.startDate}
            onChange={handleChange}
            placeholder="Start Date"
          />
          <input
            type="text"
            name="endDate"
            value={inputRequestBody.endDate}
            onChange={handleChange}
            placeholder="End Date"
          />
          <input
            type="text"
            name="interval"
            value={inputRequestBody.interval}
            onChange={handleChange}
            placeholder="Interval"
          />
          <input
            type="text"
            name="camera"
            value={inputRequestBody.camera}
            onChange={handleChange}
            placeholder="Camera"
          />
          <button type="submit">Get Timeline</button>
        </div>
      </form>

      {timeLines && (
        <div>
          <div>{timeLines[0].cameraId}</div>
          <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Start Date</th>
                <th>End Date</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {timeLines[0].record.map(
                (record: Record, recordIndex: number) => {
                  const startDateFormatted = new Date(record.start)
                    .toISOString()
                    .replace("T", " ")
                    .split(".")[0];
                  const endDateFormatted = new Date(record.end)
                    .toISOString()
                    .replace("T", " ")
                    .split(".")[0];
                  return (
                    <tr key={recordIndex}>
                      <td>{startDateFormatted}</td>
                      <td>{endDateFormatted}</td>
                      <td>
                        <button>Action</button>
                      </td>
                    </tr>
                  );
                }
              )}
            </tbody>
          </table>
          </div>
        </div>
      )}
    </>
  );
}

export default App;
