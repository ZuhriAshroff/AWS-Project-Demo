import { useState, useRef } from 'react';
import { presignUpload, uploadToS3, confirmUpload } from '../api';

type Status = 'idle' | 'uploading' | 'success' | 'error';

interface Props {
    onUploadComplete: () => void;
}

export function Upload({ onUploadComplete }: Props) {
    const [file, setFile] = useState<File | null>(null);
    const [preview, setPreview] = useState<string | null>(null);
    const [status, setStatus] = useState<Status>('idle');
    const [message, setMessage] = useState('');
    const inputRef = useRef<HTMLInputElement>(null);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const selected = e.target.files?.[0];
        if (selected) {
            setFile(selected);
            setPreview(URL.createObjectURL(selected));
            setStatus('idle');
            setMessage('');
        }
    };

    const handleUpload = async () => {
        if (!file) return;

        setStatus('uploading');
        setMessage('Getting upload URL...');

        try {
            const { id, objectKey, uploadUrl } = await presignUpload(file.name, file.type, file.size);

            setMessage('Uploading to S3...');
            await uploadToS3(uploadUrl, file);

            setMessage('Confirming upload...');
            await confirmUpload({
                id,
                objectKey,
                fileName: file.name,
                contentType: file.type,
                sizeBytes: file.size
            });

            setStatus('success');
            setMessage('Upload complete!');
            setFile(null);
            setPreview(null);
            if (inputRef.current) inputRef.current.value = '';
            onUploadComplete();
        } catch (err) {
            setStatus('error');
            setMessage(err instanceof Error ? err.message : 'Upload failed');
        }
    };

    return (
        <section>
            <h2>Upload Image</h2>
            <div className="upload-zone">
                <input
                    ref={inputRef}
                    type="file"
                    id="file-input"
                    accept="image/*"
                    onChange={handleFileChange}
                />
                <label htmlFor="file-input">
                    {file ? file.name : 'Click to select an image'}
                </label>
                {preview && <img src={preview} alt="Preview" className="preview" />}
            </div>
            <button className="btn" onClick={handleUpload} disabled={!file || status === 'uploading'}>
                {status === 'uploading' ? 'Uploading...' : 'Upload'}
            </button>
            {message && <div className={`status ${status}`}>{message}</div>}
        </section>
    );
}
