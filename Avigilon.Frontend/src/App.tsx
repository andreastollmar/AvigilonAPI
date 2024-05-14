import './App.css'
import { TimelineRequestBody } from '../types/TimelineRequestBody'
import { Timelines } from '../types/Timeline'
import { useEffect, useState } from 'react'
import FetchTimeline from './API/fetchTimeline'

function App() {
  const [inputRequestBody, setInputRequestBody] = useState<TimelineRequestBody>({
    startDate: '',
    endDate: '',
    interval: '',
    camera: ''
  });
  const [timeLines, setTimelines] = useState<Timelines[] | null>(null);
  const [errorMessage, setErrorMessage] = useState();

  function handleChange(event: React.ChangeEvent<HTMLInputElement>) {
    const { name, value } = event.target;
    setInputRequestBody(prevState => ({
      ...prevState,
      [name]: value
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
        const response = await fetch("https://localhost:44390/getList", requestOptions);
  
        if (!response.ok) {
          const errorFromFetch = await response.json();
          setErrorMessage(errorFromFetch);
        } else {
          const data = (await response.json()) as Timelines[];
          setTimelines(data);
        }
      };
      fetchData();
      console.log("Running get Timeline api call") ;  
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

      {timeLines &&
        timeLines.map((cameraRecord: Timelines, index: number) => (
          <div key={index}>{cameraRecord.timelines[index].cameraId} - {cameraRecord.timelines[index].record[0].start} - {cameraRecord.timelines[index].record[0].end}</div>
        ))
      }
    </>
  )
}

export default App
