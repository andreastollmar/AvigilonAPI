import "./App.css";
import { useState } from "react";
import { MediaRequestContract } from "../types/MediaRequestContract";
import { SaveMediaFromApi } from "./API/saveMediaFromApi";

function App() {
  const [mediaRequest, setMediaRequest] = useState<MediaRequestContract>({
    camera: '',
    isImg: false,
    requestBody: [{ date: '', time: '' }],
  });

  const [successMsg, setSuccessMsg] = useState<string | undefined>();
  const [errorMsg, setErrorMsg] = useState<string | undefined>();

  const handleAddRow = () => {
    setMediaRequest(prev => ({
      ...prev,
      requestBody: [...prev.requestBody, { date: '', time: '' }]
    }));
  };

  const handleRemoveRow = (index: number) => {
    setMediaRequest(prev => ({
      ...prev,
      requestBody: prev.requestBody.filter((_, idx) => idx !== index)
    }));
  };

  const handleChange = (index: number, event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setMediaRequest(prev => ({
      ...prev,
      requestBody: prev.requestBody.map((item, idx) => 
        idx === index ? { ...item, [name]: value } : item
      )
    }));
  };

  const handleCheckboxChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setMediaRequest(prev => ({
      ...prev,
      isImg: event.target.checked
    }));
  };

  const handleCameraChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setMediaRequest(prev => ({
      ...prev,
      camera: event.target.value
    }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    
    const result = await SaveMediaFromApi(mediaRequest);
    if (result.errorMsg) {
      setErrorMsg(result.errorMsg);
      setSuccessMsg(undefined);
    } else if (result.successMsg) {
      setSuccessMsg(result.successMsg);
      setErrorMsg(undefined);
    }
  };

  return (
    <>
      <form onSubmit={handleSubmit}>
        {mediaRequest.requestBody.map((input, index) => (
          <div key={index}>
            <input
              type="text"
              name="date"
              value={input.date}
              onChange={(event) => handleChange(index, event)}
              placeholder="Date"
            />
            <input
              type="text"
              name="time"
              value={input.time}
              onChange={(event) => handleChange(index, event)}
              placeholder="Time"
            />
            <button type="button" onClick={() => handleRemoveRow(index)}>
              Remove
            </button>
          </div>
        ))}
        <button type="button" onClick={handleAddRow}>Add Row</button>
        <div>
          <input
            type="checkbox"
            name="isImg"
            checked={mediaRequest.isImg}
            onChange={handleCheckboxChange}
          />
          <label htmlFor="isImg">Img?</label>
        </div>
        <div>
          <input
            type="text"
            name="camera"
            value={mediaRequest.camera}
            onChange={handleCameraChange}
            placeholder="Camera"
          />
        </div>
        <button type="submit">Save media files</button>
      </form>

      {/* <div>
        {successMsg && <div>{successMsg}</div>}
        {errorMsg && <div>{errorMsg}</div>}
      </div> */}
    </>
  );
}

export default App;
