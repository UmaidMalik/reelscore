const apiBaseUrl = import.meta.env.VITE_API_BASE_URL

if (!apiBaseUrl) {
  throw new Error('VITE_API_BASE_URL is not configured.')
}

type ApiErrorBody = {
  title?: string
  detail?: string
  message?: string
}

export class ApiError extends Error {
  public readonly status: number
  public readonly body?: ApiErrorBody

  public constructor(
    status: number,
    message: string,
    body?: ApiErrorBody,
  ) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.body = body
  }
}

export async function apiRequest<T>(
  path: string,
  options?: RequestInit,
): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers: {
      Accept: 'application/json',
      ...options?.headers,
    },
  })

  if (!response.ok) {
    let body: ApiErrorBody | undefined

    try {
      body = (await response.json()) as ApiErrorBody
    } catch {
      body = undefined
    }

    throw new ApiError(
      response.status,
      body?.detail ??
        body?.message ??
        body?.title ??
        `Request failed with status ${response.status}.`,
      body,
    )
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}