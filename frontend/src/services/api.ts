import axios from 'axios'

export interface ProcessRequest {
  code: string
  action: 'explain' | 'fix' | 'optimize'
}

export interface ProcessResponse {
  result: string
  action: string
  timestamp: string
}

const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

export const aiService = {
  processCode: async (data: ProcessRequest): Promise<ProcessResponse> => {
    const response = await api.post<ProcessResponse>('/ai/process', data)
    return response.data
  },
}